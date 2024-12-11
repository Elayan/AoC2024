using System.Text;
using AoCTools.Loggers;
using AoCTools.Numbers;

namespace AoC2024.Structures;

public class Cluster
{
    public int Size { get; set; }
    public long Label { get; set; }

    public long GetChecksum(int offset)
    {
        if (Label <= 0)
            return 0L;
        
        // Because total checksum is sum first integers * cell[id].Label
        // the checksum for a cluster
        // is the sum between offset and (offset + Size - 1)
        // multiplied by Label
        return NonoMath.SumIntegersBetween(offset, offset + Size - 1) * Label;
    }

    public override string ToString()
    {
        return Size.ToString();
    }
}

public class Disk
{
    public List<Cluster> Clusters { get; private set; } = new();
    public List<Cluster> DefragmentedClusters { get; private set; } = new();

    public Disk(string line)
    {
        long curIndex = 0;
        bool isFreeSpace = false;
        foreach (var c in line)
        {
            Clusters.Add(new Cluster { Size = c - 48, Label = isFreeSpace ? -1 : curIndex++ });
            isFreeSpace = !isFreeSpace;
        }
    }

    public void Defragment()
    {
        DefragmentedClusters.Clear();
        var clustersToProcess = new List<Cluster>(Clusters);

        var firstCluster = GetNextCluster(ref clustersToProcess, true, false);
        var lastCluster = GetNextCluster(ref clustersToProcess, false, true);
        if (firstCluster == null || lastCluster == null)
            return;
        
        while (true)
        {
            if (firstCluster.Label != -1)
            {
                Logger.Log($"Preserve cluster {firstCluster.Label} of size {firstCluster.Size}");
                DefragmentedClusters.Add(firstCluster);
                firstCluster = GetNextCluster(ref clustersToProcess, true, false);
                if (firstCluster == null)
                    break;

                continue;
            }
            
            Logger.Log($"Filling up free cluster of size {firstCluster.Size}");
            while (firstCluster.Size > 0)
            {
                if (lastCluster.Size > firstCluster.Size)
                {
                    DefragmentedClusters.Add(new Cluster { Size = firstCluster.Size, Label = lastCluster.Label});
                    var memoLastClusterSize = lastCluster.Size;
                    lastCluster.Size -= firstCluster.Size;
                    Logger.Log($">> adding part of cluster {lastCluster.Label}, size {firstCluster.Size} out of {memoLastClusterSize} (remaining {lastCluster.Size})");
                    firstCluster.Size = 0;
                }
                else
                {
                    DefragmentedClusters.Add(new Cluster { Size = lastCluster.Size, Label = lastCluster.Label});
                    firstCluster.Size -= lastCluster.Size;
                    Logger.Log($">> adding entire cluster {lastCluster.Label} of size {lastCluster.Size}. Still {firstCluster.Size} to fill.");
                    lastCluster = GetNextCluster(ref clustersToProcess, false, true);
                    if (lastCluster == null)
                        break;
                }
            }

            firstCluster = GetNextCluster(ref clustersToProcess, true, false);
            if (firstCluster == null)
            {
                if (lastCluster != null)
                {
                    var lastDefragmentedCluster = DefragmentedClusters.Last();
                    if (lastCluster.Label == lastDefragmentedCluster.Label)
                    {
                        lastDefragmentedCluster.Size += lastCluster.Size;
                        Logger.Log($"Merging last cluster {lastCluster.Label} of size {lastCluster.Size} => size total {lastDefragmentedCluster.Size}");
                    }
                    else
                    {
                        DefragmentedClusters.Add(lastCluster);
                        Logger.Log($"Adding last cluster {lastCluster.Label} of size {lastCluster.Size}");
                    }
                }

                break;
            }
        }
    }

    public void RearrangeFiles()
    {
        DefragmentedClusters = new List<Cluster>(Clusters);
        for (int index = DefragmentedClusters.Count - 1; index > 1; index--)
        {
            if (DefragmentedClusters[index].Label == -1)
            {
                Logger.Log("We don't move free clusters, skipping.");
                continue;
            }
            
            var lastCluster = DefragmentedClusters[index];
            Logger.Log($"Trying to find a spot for cluster {lastCluster.Label} of size {lastCluster.Size}");

            bool clusterMoved = false;
            for (int i = 0; !clusterMoved && i < index; i++)
            {
                if (DefragmentedClusters[i].Label == -1 && DefragmentedClusters[i].Size >= lastCluster.Size)
                {
                    clusterMoved = true;
                    Logger.Log($"Found a free spot of size {DefragmentedClusters[i].Size}");
                    
                    DefragmentedClusters.RemoveAt(index);
                    DefragmentedClusters.Insert(index, new Cluster { Label = -1, Size = lastCluster.Size });
                    Logger.Log("> Last cluster replaced by free space of same size.");

                    var freeClusterSize = DefragmentedClusters[i].Size;
                    DefragmentedClusters.RemoveAt(i);
                    
                    DefragmentedClusters.Insert(i, lastCluster);
                    freeClusterSize -= lastCluster.Size;
                    Logger.Log($"> Last cluster inserted in place of free spot. Free space left: {freeClusterSize}.");

                    if (freeClusterSize > 0)
                    {
                        DefragmentedClusters.Insert(i + 1, new Cluster { Label = -1, Size = freeClusterSize });
                        index++;
                        Logger.Log($"> Free space of size {freeClusterSize} inserted after cluster.");
                    }
                    
                    Logger.Log(ClustersToDevelopedString(DefragmentedClusters));
                }
            }

            if (!clusterMoved)
                Logger.Log("No space found to move this cluster.");
        }
    }

    private Cluster GetNextCluster(ref List<Cluster> clusters, bool atBeginning, bool skipFreeSpace)
    {
        var atBeginningStr = atBeginning ? "[FIRST] " : "[LAST] ";
        while (true)
        {
            var cluster = atBeginning ? clusters.FirstOrDefault() : clusters.LastOrDefault();
            clusters.Remove(cluster);

            if (cluster == null)
            {
                Logger.Log($"{atBeginningStr}No more cluster.");
                return null;
            }

            if (skipFreeSpace && cluster.Label == -1)
            {
                Logger.Log($"{atBeginningStr}Ignoring free cluster of size {cluster.Size}");
                continue;
            }

            if (cluster.Size > 0)
            {
                Logger.Log($"{atBeginningStr}Taking cluster {cluster.Label} of size {cluster.Size}");
                return cluster;
            }
            
            Logger.Log($"{atBeginningStr}Cluster {cluster.Label} is empty, ignoring.");
        }
    }

    public long GetDefragmentedChecksum()
    {
        long checksum = 0L;

        int offset = 0;
        foreach (var cluster in DefragmentedClusters)
        {
            var clusterChecksum = cluster.GetChecksum(offset);
            offset += cluster.Size;
            checksum += clusterChecksum;
            Logger.Log($"Cluster {cluster.Label} of size {cluster.Size} checksum with offset {offset} = {clusterChecksum} => checksum = {checksum}");
        }

        return checksum;
    }

    public override string ToString()
    {
        return ClustersToString(Clusters, "DISK");
    }

    public string ToDefragmentedString()
    {
        return ClustersToString(DefragmentedClusters, "DEFRAGMENTED DISK");
    }

    private string ClustersToString(List<Cluster> clusters, string title)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"=== {title} ===");
        sb.AppendLine(string.Join("", clusters));
        return sb.ToString();
    }

    public string ToDevelopedString()
    {
        return ClustersToDevelopedString(Clusters, "DEVELOPED DISK");
    }

    public string ToDefragmentedDevelopedString()
    {
        return ClustersToDevelopedString(DefragmentedClusters, "DEFRAGMENTED DEVELOPED DISK");
    }

    private string ClustersToDevelopedString(List<Cluster> clusters, string title)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"=== {title} ===");
        sb.AppendLine(ClustersToDevelopedString(clusters));
        return sb.ToString();
    }

    private string ClustersToDevelopedString(List<Cluster> clusters)
    {
        return string.Join("", clusters.Select(c => string.Join("", Enumerable.Repeat(c.Label == -1 ? "." : c.Label.ToString(), c.Size))));
    }
}