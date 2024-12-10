﻿using System.Text;
using AoCTools.Loggers;
using AoCTools.Numbers;

namespace AoC2024.Structures;

public class Cluster
{
    public int Size { get; set; }
    public long Label { get; set; }

    public long GetChecksum(int offset)
    {
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
        var clustersToProcess = new List<Cluster>(Clusters.Select(c => new Cluster { Size = c.Size, Label = c.Label }));

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

    [Obsolete]
    public void Defragment_Old()
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

            if (Clusters[ite].Size == 0)
            {
                Logger.Log($"Cluster {ite} is empty, skipping.");
                ite++;
                continue;
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

    public long GetDefragmentedChecksum_Raw()
    {
        var developed = ClustersToDevelopedString(DefragmentedClusters);
        var checksum = 0L;
        for (int i = 0; i < developed.Length; i++)
        {
            checksum += i * (developed[i] - 48);
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