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
        // checksum = 0*Label + 1*Label + 2*Label + ... + (Size-1)*Label
        // checksum = (0 + 1 + 2 + ... + (Size-1)) * Label
        // checksum = Somme Des (Size - 1) Premiers Entiers * Label
        return (NonoMath.SumFirstIntegers(offset + (long)Size - 1L) - NonoMath.SumFirstIntegers(offset - 1L)) * Label;
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
        int ite = 0;
        int lastFileClusterIdx = Clusters.Count - 1;
        if (Clusters[lastFileClusterIdx].Label == -1)
        {
            // shouldn't happen, but better be sure
            Logger.Log("Last cluster is empty... ok?");
            lastFileClusterIdx--;
        }
        int lastFileClusterSize = Clusters[lastFileClusterIdx].Size;
        
        while (true)
        {
            if (ite >= lastFileClusterIdx)
            {
                var lastDefragmentedCluster = DefragmentedClusters.Last();
                if (lastDefragmentedCluster.Label == Clusters[lastFileClusterIdx].Label)
                {
                    lastDefragmentedCluster.Size += lastFileClusterSize;
                    Logger.Log($"Merging what's left of file cluster {lastDefragmentedCluster.Label}, size {lastFileClusterSize} => total {lastDefragmentedCluster.Size}");
                }
                else
                {
                    DefragmentedClusters.Add(new Cluster { Size = lastFileClusterSize, Label = Clusters[ite].Label });
                    Logger.Log($"Preserving what's left of file cluster {Clusters[ite].Label}, size {lastFileClusterSize}");
                }
                
                Logger.Log("End of defragmentation!");
                break;
            }
            
            if (Clusters[ite].Label != -1)
            {
                DefragmentedClusters.Add(Clusters[ite]);
                Logger.Log($"Preserving file cluster {Clusters[ite].Label} of size {Clusters[ite].Size}");
                ite++;
                continue;
            }
            
            var curFreeClusterSize = Clusters[ite].Size;
            Logger.Log($"Filling up free cluster of size {curFreeClusterSize}");
            while (lastFileClusterSize < curFreeClusterSize)
            {
                DefragmentedClusters.Add(new Cluster { Size = lastFileClusterSize, Label = Clusters[lastFileClusterIdx].Label});
                curFreeClusterSize -= lastFileClusterSize;
                Logger.Log($"> adding entire cluster {Clusters[lastFileClusterIdx].Label} of size {lastFileClusterSize} => remaining: {curFreeClusterSize}");
                
                lastFileClusterIdx -= 2;
                lastFileClusterSize = Clusters[lastFileClusterIdx].Size;
            }
            
            DefragmentedClusters.Add(new Cluster { Size = curFreeClusterSize, Label = Clusters[lastFileClusterIdx].Label });
            var memoLastSize = lastFileClusterSize;
            lastFileClusterSize -= curFreeClusterSize;
            Logger.Log($"> adding part cluster {Clusters[lastFileClusterIdx].Label}, {curFreeClusterSize} out of {memoLastSize}, leaving {lastFileClusterSize}");

            if (lastFileClusterSize == 0)
            {
                Logger.Log($"Cluster {Clusters[lastFileClusterIdx].Label} is now empty, skipping.");
                lastFileClusterIdx -= 2;
                lastFileClusterSize = Clusters[lastFileClusterIdx].Size;
            }

            ite++;
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
        sb.AppendLine(string.Join("", clusters.Select(c => string.Join("", Enumerable.Repeat(c.Label == -1 ? "." : c.Label.ToString(), c.Size)))));
        return sb.ToString();
    }
}