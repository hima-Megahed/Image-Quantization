using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    public class Graph
    {
           public List<RGBPixel> vertex;
           public List<Edge> edges;
           public Graph()
           {
               vertex=new List<RGBPixel>();
               edges = new List<Edge>();
           }
    }
}
