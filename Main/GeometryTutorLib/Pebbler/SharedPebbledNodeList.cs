﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GeometryTutorLib.Pebbler
{
    //
    // This code is based on the MSDN example for Producer / Consumer in C#
    //
    public class SharedPebbledNodeList
    {
        // This is shared data structure between the producer (Pebbler) and consumer (path generator) threads
        private List<PebblerHyperEdge> edgeList;

        public SharedPebbledNodeList()
        {
            edgeList = new List<PebblerHyperEdge>();
        }

        // State flag
        private bool writerFlag = false;  // on when writing
        private bool readerFlag = false;  // on when reading
        private bool writingComplete = false;

        public void SetWritingComplete()
        {
            writingComplete = true;
        }

        public bool IsReadingAndWritingComplete()
        {
            return writingComplete && !edgeList.Any();
        }

        //
        // Consumer method
        //
        public PebblerHyperEdge ReadEdge()
        {
            // If writing is known to be done and the list is empty, this is an error
            if (IsReadingAndWritingComplete()) return null;

            PebblerHyperEdge readEdge = null;

            // Enter synchronization block
            lock (this)
            {
                // Wait until WriteEdge produces OR is done producing a new edge
                if (writerFlag || !edgeList.Any())
                {
                    try
                    {
                        // Waits for the Monitor.Pulse in WriteEdge
                        Monitor.Wait(this);

                        //CTA: Should add a timeout in case nothing is ever written...
                    }
                    catch (SynchronizationLockException e)
                    {
                        System.Diagnostics.Debug.WriteLine(e);
                    }
                }

                readerFlag = true;

                // Consume the edge
                readEdge = edgeList[0];
                edgeList.RemoveAt(0);

                // Reset the state flag to say producing is done.
                readerFlag = false;

                // Pulse tells WriteEdge that ReadEdge is done.
                Monitor.Pulse(this);
            }

            return readEdge;
        }

        //
        // Producer method
        //
        public void WriteEdge(PebblerHyperEdge edgeToWrite)
        {
            // Enter synchronization block
            lock (this)
            {
                if (readerFlag)
                {   
                    // Wait until ReadEdge is done consuming.
                    try
                    {
                        // Wait for the Monitor.Pulse in Read
                        Monitor.Wait(this);
                    }
                    catch (SynchronizationLockException e)
                    {
                        System.Diagnostics.Debug.WriteLine(e);
                    }
                }

                writerFlag = true;

                // Produce
                edgeList.Add(edgeToWrite);

                // Reset the state flag to say producing is done.
                writerFlag = false;

                // Pulse tells Read that Write is complete. 
                Monitor.Pulse(this);
            }
        }
    }
}
