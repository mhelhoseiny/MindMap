using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OurMindMapOntology;
using MindMapMeaningRepresentation;
using SyntacticAnalyzer;
using mmTMR;

namespace MultilevelGenerator
{
    public class WeightBasedPartitioner
    {
        public Dictionary<int, List<List<int>>> KGroups = new Dictionary<int, List<List<int>>>();
        Dictionary<int, double> KVariance = new Dictionary<int, double>();
        public Dictionary<int, List<double>> KCentroids = new Dictionary<int, List<double>>();
        public List<Frame> Frames;
        List<double> FramesWeights;
        List<int> indexes;
        public int optimalK;
        public int MaximumK;
        double threshold = 0.02;

        public WeightBasedPartitioner(List<Frame> Frames, List<double> FramesWeights)
        {
            this.Frames = Frames;
            this.FramesWeights = FramesWeights;
            this.indexes = new List<int>();
            fillIndexes();
        }

        private void fillIndexes()
        {
            for (int i = 0; i < this.Frames.Count; i++)
            {
                this.indexes.Add(i);
            }
        }

        public List<List<Frame>> Partition()
        {

            optimalK = 1;

            getMaximumK();

            List<List<Frame>> Clusters;
            for (int K = this.MaximumK; K > 0; K--)
            {
                KMeans kmeans = new KMeans(K, this.indexes, this.FramesWeights);
                KGroups.Add(K, kmeans.Run());
                KVariance.Add(K, kmeans.GetAverageVariance());
                KCentroids.Add(K, kmeans.getCentroids());
            }

            normalizeVariances();

            if (MaximumK != 1)
            {
                for (int K = this.MaximumK; K > 0; K--)
                {
                    if (Math.Abs(KVariance[K] - KVariance[K - 1]) > this.threshold)
                    {
                        optimalK = K;
                        break;
                    }
                }
            }

            else
            {
                optimalK = MaximumK;
            }

            Clusters = getClusters(optimalK);

            return Clusters;
        }

        private void getMaximumK()
        {
            List<double> K = new List<double>();

            for (int i = 0; i < this.FramesWeights.Count; i++)
            {
                if (K.Contains(this.FramesWeights[i]) == false)
                    K.Add(this.FramesWeights[i]);
            }

            MaximumK = K.Count;
        }

        private void normalize_FrameWeights()
        {
            double minimum = double.MaxValue;
            for (int i = 0; i < this.FramesWeights.Count; i++)
            {

                if (this.FramesWeights[i] < minimum)
                {
                    minimum = this.FramesWeights[i];
                }
            }

            double maximum = 0;

            for (int i = 0; i < this.FramesWeights.Count; i++)
            {
                this.FramesWeights[i] -= minimum;
                if (this.FramesWeights[i] > maximum)
                {
                    maximum = this.FramesWeights[i];
                }
            }

            for (int i = 0; i < this.FramesWeights.Count; i++)
            {
                this.FramesWeights[i] = this.FramesWeights[i] / maximum;
            }
        }

        private void normalizeVariances()
        {
            double minimum = double.MaxValue;
            for (int i = this.KVariance.Count; i > 0; i--)
            {
                if (i != 1)
                {
                    this.KVariance[i] = (this.KVariance[i] - this.KVariance[i - 1]);
                }
                else if (i == 1)
                {
                    this.KVariance[1] = 0;
                }
                if (this.KVariance[i] <= minimum)
                {
                    minimum = this.KVariance[i];
                }
            }

            double maximum = 0;

            for (int i = this.KVariance.Count; i > 0; i--)
            {
                this.KVariance[i] = this.KVariance[i] - minimum;
                if (this.KVariance[i] >= maximum)
                {
                    maximum = this.KVariance[i];
                }
            }

            for (int i = this.KVariance.Count; i > 0; i--)
            {
                this.KVariance[i] = this.KVariance[i] / maximum;

            }

        }

        private List<List<Frame>> getClusters(int optimalK)
        {
            List<List<Frame>> Clusters = new List<List<Frame>>();

            foreach (List<int> list in this.KGroups[optimalK])
            {
                List<Frame> FrameList = new List<Frame>();
                foreach (int i in list)
                {
                    FrameList.Add(this.Frames[i]);
                }
                Clusters.Add(FrameList);
            }

            return Clusters;
        }

       
    }
}
