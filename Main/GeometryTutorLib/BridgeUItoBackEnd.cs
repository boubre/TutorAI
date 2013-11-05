using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib;
using System.Diagnostics;
using System.Threading;

namespace GeometryTutorLib
{
    public class BridgeUItoBackEnd
    {
        public static void AnalyzeFigure(List<ConcreteAbstractSyntax.GroundedClause> figure)
        {
            //
            // Begin timing code
            //
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing
            stopwatch.Start();


            //
            // Begin main analysis code
            //
            GenericInstantiator.Instantiator instantiator = new GenericInstantiator.Instantiator();

            Hypergraph.Hypergraph<ConcreteAbstractSyntax.GroundedClause, int> graph = instantiator.Instantiate(figure);

            graph.DebugDumpClauses();

            Pebbler.PathGenerator pathGenerator = null;

            // Create the Pebbler version of the hypergraph (all integer representation)
            Pebbler.PebblerHypergraph<ConcreteAbstractSyntax.GroundedClause> pebblerGraph = graph.GetPebblerHypergraph();

            // This is the shared worklist for generating paths for when new nodes are pebbled 
            Pebbler.SharedPebbledNodeList sharedData = new Pebbler.SharedPebbledNodeList();

            // Create and / or indicate these two objects work on the shared list object
            pathGenerator = new Pebbler.PathGenerator(pebblerGraph.NumVertices(), sharedData);
            pebblerGraph.SetSharedList(sharedData);

            // For testing purposes, pebble from this start node only
            int start = 0;
            int stop = 37; // figure.Count;
            stop = stop < graph.Size() ? stop : graph.Size() - 1; 
            List<int> srcs = new List<int>();
            for (int i = start; i <= stop; i++)
            {
                srcs.Add(i);
            }

            // Set the nodes to begin pebbling
            pebblerGraph.SetSourceNodes(srcs);

            // Create producer and consumer threads for problem path / solution generation
            Thread pebblerProducer = new Thread(new ThreadStart(pebblerGraph.GenerateAllPaths));
            Thread pathGeneratorConsumer = new Thread(new ThreadStart(pathGenerator.GenerateAllPaths));

            // Start the threads
            try
            {
                pebblerProducer.Start();
                // Allow production to occur so consuming is constant
                //Thread.Sleep(500);
                pathGeneratorConsumer.Start();

                // Join both threads with no timeout
                pebblerProducer.Join();
                pathGeneratorConsumer.Join();
            }
            catch (ThreadStateException e)
            {
                Debug.WriteLine(e);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            // Pebble
            //pebblerGraph.GenerateAllPaths();

            // Generate the paths found.
            //Debug.WriteLine("All Paths:");
            //pathGenerator.GenerateAllPaths();

            //
            // End Analysis Code
            //

            // Stop timing
            stopwatch.Stop();

            Debug.WriteLine("Vertices after pebbling:");
            for (int i = 0; i < pebblerGraph.vertices.Length; i++)
            {
                Debug.WriteLine(pebblerGraph.vertices[i].id + ": pebbled(" + pebblerGraph.vertices[i].pebbled + ")");
            }

            TimeSpan ts = stopwatch.Elapsed;
            // Format and display the TimeSpan value. 
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                               ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            Debug.WriteLine("Length of time to compute all paths: " + elapsedTime);
            Debug.WriteLine("Number of Unique Paths / Problems: " + pathGenerator.GetPaths().Count);
            //
            // End timing Code
            //
        }
    }
}
