using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
namespace ImageQuantization
{
    public struct Bin_node
    {
        public double weight;
         public int node;
    }
    class ALL_FUNCTIONS
    {
        private static Bin_node[] Bin_heap;
        private static int size_Bin, pos, last;
        private static double cost; 
        private static Bin_node tmp;
        private static Bin_node pmp;
        private static Bin_node tmp_swap;
        private static Bin_node val;
        private static Bin_node new_Bin;
        private static int Max_Size = 1000;
        private static int[] root = new int[Max_Size];
        private static int[] cnt = new int[Max_Size];
        private static HashSet<RGBPixel> nodes = new HashSet<RGBPixel>();
        private static Dictionary<RGBPixel, HashSet<RGBPixel>> adjList = new Dictionary<RGBPixel, HashSet<RGBPixel>>();
        //private static List<RGBPixel> tmpGroup = new List<RGBPixel>();
        private static HashSet<RGBPixel> vis;
        private static List<RGBPixel> PaletteColor;
        private static Bin_node[] From_To;
        private static Graph tree;
        private static List<RGBPixel> distinct;
        public static int get_distinct(ref RGBPixel[,] Buffer)
        {
            bool[, ,] visited_buffer = new bool[256, 256, 256];
            RGBPixel new_one;
            distinct = new List<RGBPixel>();
            
            int rw = Buffer.GetLength(0);
            int cl = Buffer.GetLength(1);
            for (int row = 0; row < rw; row++)
            {
                for (int col = 0; col < cl; col++)
                {
                    new_one = Buffer[row, col];
                    if (visited_buffer[new_one.red, new_one.green, new_one.blue] == false)
                    {
                        visited_buffer[new_one.red, new_one.green, new_one.blue] = true;
                        distinct.Add(new_one);
                    }
                }
            }
            MessageBox.Show(distinct.Count.ToString());
            return distinct.Count;
        }

        public static double generate_dif(ref int i, ref int j)
        {
            double dif;
            RGBPixel new_one1 = distinct[i], new_one2 = distinct[j];
            dif = (double)Math.Sqrt((new_one1.red - new_one2.red) * (new_one1.red - new_one2.red) + (new_one1.blue - new_one2.blue) * (new_one1.blue - new_one2.blue) + (new_one1.green - new_one2.green) * (new_one1.green - new_one2.green));
            return dif;
        }
        public static Bin_node get_min(ref Bin_node a, ref Bin_node b, ref int i)
        {
            if (a.weight < b.weight)
            {
                pos = i;
                return a;
            }
            else if (a.weight > b.weight)
            {
                return b;
            }
            else
            {
                if (a.node < b.node)
                {
                    pos = i;
                    return a;
                }
                else
                    return b;
            }
        }
        public static void swap_Bin(ref int i, ref int j)
        {
            tmp_swap = Bin_heap[i];
            Bin_heap[i] = Bin_heap[j];
            Bin_heap[j] = tmp_swap;
        }
        public static void Set_Mst()
        {
            size_Bin = distinct.Count;
            Bin_heap = new Bin_node[size_Bin];
            From_To = new Bin_node[size_Bin];
            Bin_heap[0].weight = 0.0; Bin_heap[0].node = 0;
            pos = 0;
            tmp = Bin_heap[0];
            while (size_Bin != 0)
            {
                last = size_Bin - 1;
                swap_Bin(ref pos, ref last);
                pmp.weight = 10000000000.0; pmp.node = 1000000000;
                for (int i = 0; i < size_Bin - 1; i++)
                {
                    val = Bin_heap[i];
                    if (tmp.node != 0)
                        cost = generate_dif(ref tmp.node, ref val.node);
                    if (tmp.node == 0)   // start node
                    {
                        if (i != 0)
                        {
                            cost = generate_dif(ref tmp.node, ref i);
                            From_To[i].weight = cost;
                            From_To[i].node = tmp.node;
                            Bin_heap[i].weight = cost;
                            Bin_heap[i].node = i;
                            pmp = get_min(ref Bin_heap[i], ref pmp, ref i);
                        }
                        else
                        {
                            cost = generate_dif(ref tmp.node, ref last);
                            From_To[last].weight = cost;
                            From_To[last].node = tmp.node;
                            Bin_heap[i].weight = cost;
                            Bin_heap[i].node = last;
                            pmp = get_min(ref Bin_heap[i], ref pmp, ref i);
                        }
                    }
                    else if (cost < val.weight)
                    {
                        From_To[val.node].weight = cost;
                        From_To[val.node].node = tmp.node;
                        Bin_heap[i].weight = cost;
                        pmp = get_min(ref Bin_heap[i], ref pmp, ref i);
                    }
                    else
                    {
                        pmp = get_min(ref Bin_heap[i], ref pmp, ref  i);
                    }

                }
                tmp = pmp;
                size_Bin--;
            }
            double sum = 0;
            for (int i = 1; i < distinct.Count; i++)
            {
                sum += From_To[i].weight;
             //   MessageBox.Show(From_To[i].weight.ToString());
            }
            MessageBox.Show(sum.ToString());
        }
        public static void Build_tree()
        {
            int cnt = From_To.Length;
            tree = new Graph();
            tree.vertex = distinct;

            for (int i = 1; i < cnt; i++)
            {
                Edge edge = new Edge();
                edge.From = distinct[i];
                edge.To = distinct[From_To[i].node];
                edge.weight = From_To[i].weight;
                //MessageBox.Show(edge.From.ToString() + " " + edge.To.ToString() + " " + edge.weight.ToString());
                tree.edges.Add(edge);
            }

        }
        //Build an adjacency list from MST
        public static void buildAdjacencyList()
        {

            for (int i = 0; i < distinct.Count; i++)
            {
                adjList[distinct[i]] = new HashSet<RGBPixel>();
            }

            for (int i = 0; i < tree.edges.Count; i++)
            {
                adjList[tree.edges[i].From].Add(tree.edges[i].To);
                adjList[tree.edges[i].To].Add(tree.edges[i].From);
                nodes.Add(tree.edges[i].To);
                nodes.Add(tree.edges[i].From);
            }
        }

        //Cut edges of MST in non-increasing order then cut k-1 edges from it
        public static void CutEdges(int k)
        {
            //tree.edges.Sort((Edge E1, Edge E2) => -1 * E1.weight.CompareTo(E2.weight));
            for (int i = 0; i < k - 1; i++)
            {
                double mx = 0.0;
                int idx = 0;
                for (int j = 0; j < tree.edges.Count; j++)
                {
                    if (tree.edges[j].weight > mx)
                    {
                        mx = tree.edges[j].weight;
                        idx = j;
                    }
                }
                RGBPixel node1 = tree.edges[idx].From;
                RGBPixel node2 = tree.edges[idx].To;
                tree.edges[idx].weight = -1;
                adjList[node1].Remove(node2);
                adjList[node2].Remove(node1);
            }
        }

        //Depth First Search
        public static void DFS(RGBPixel start,  List<RGBPixel> tmpGroup)
        {

            nodes.Remove(start);
            tmpGroup.Add(start);
            foreach (var item in adjList[start])
            {
                if (nodes.Contains(item))
                {
                    nodes.Remove(item);
                    DFS(item, tmpGroup);
                }
            }
        }

        //clustring K colors
        public static List<List<RGBPixel>> cluster(int k)
        {
            List<List<RGBPixel>> groups = new List<List<RGBPixel>>();
            for (int i = 0; i < k; i++)
            {
                List<RGBPixel> tmpGroup = new List<RGBPixel>();
                DFS(nodes.First(), tmpGroup);
                groups.Add(tmpGroup);
            }
            return groups;
        }

        public static int getK(int k)
        {
            return k;
        }

        // get palette of each group

        public static void GetPalette(ref List<List<RGBPixel>> groups)
        {
            PaletteColor = new List<RGBPixel>();
            RGBPixel colorG = new RGBPixel();
            for (int i = 0; i < groups.Count; i++)
            {
                colorG.red = 0;
                colorG.blue = 0;
                colorG.green = 0;
                int groupSize=groups[i].Count;
                for (int j = 0; j < groupSize; j++)
                {
                    colorG.red += groups[i][j].red;
                    colorG.blue += groups[i][j].blue;
                    colorG.green += groups[i][j].green;
                }
                colorG.red /= groups[i].Count;
                colorG.blue /= groups[i].Count;
                colorG.green /= groups[i].Count;
                PaletteColor.Add(colorG);
            }
        }

        // get the average color of each group colors

        public static RGBPixel[, ,] UpdateColor(ref List<List<RGBPixel>> groups)
        {
            RGBPixel[, ,] Update = new RGBPixel[256, 256, 256];
            for (int i = 0; i < groups.Count; i++)
            {
                int groupSize = groups[i].Count;
                for (int j = 0; j < groupSize; j++)
                {
                    Update[groups[i][j].red, groups[i][j].blue, groups[i][j].green] = PaletteColor[i];
                }
            }
            return Update;
        }
        // change group's coloros to its average color

        public static void UpdatedMatrix( RGBPixel[, ,] Update, RGBPixel[,] Matrix)
        {
            int row = Matrix.GetLength(0);
            int col = Matrix.GetLength(1);
           
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    Matrix[i, j] = Update[Matrix[i,j].red, Matrix[i,j].blue, Matrix[i,j].green];   
                }
            }
        }
    }

}