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
    public class KMeans
    {
        int k;
        List<int> indexes;
        List<double> Weights;
        List<double> Centroids;
        List<List<int>> Clusters;
        Random random;

        public KMeans(int K, List<int> Indexes, List<double> Weights)
        {
            this.random = new Random();
            this.Clusters = new List<List<int>>();
            this.Centroids = new List<double>();
            this.Weights = new List<double>();
            this.indexes = Indexes;
            this.k = K;
            for (int i = 0; i < Indexes.Count; i++)
            {
                this.Weights.Add(Weights[i]);
            }
        }

        public List<List<int>> Run()
        {
            List<double> NewCentroids;

            initializeCentroids();

            double MinimumDistance;
            int ClusterIndex = -1;

            do
            {
                clearClusters();

                for (int i = 0; i < this.indexes.Count; i++)
                {
                    MinimumDistance = double.MaxValue;

                    for (int j = 0; j < this.k; j++)
                    {
                        double distance = getDistance(this.Weights[i],Centroids[j]);
                        if(distance < MinimumDistance) 
                        {
                            MinimumDistance = distance;
                            ClusterIndex = j;
                        }
                    }

                    Clusters[ClusterIndex].Add(this.indexes[i]);
                    
                }

                

                NewCentroids = getCentroids();
            } while (centroidsChanged(NewCentroids) == true);
            return Clusters;
        }

        private void initializeCentroids()
        {
            int index = random.Next(0, this.indexes.Count - 1);
            Clusters.Add(new List<int>());
            this.Centroids.Add(this.Weights[index]);

            while (this.Centroids.Count < this.k)
            {
                double maxProbability = 0;
                int chosenindex = -1;

                for (int i = 0; i < this.indexes.Count; i++)
                {
                    double probability = 0;
                    double numerator = 0, denominator = 0;
                    if (this.Centroids.Contains(this.Weights[i]) == false)
                    {
                        double centroid = getNearestCentroid(i);
                        numerator = Math.Pow(getDistance(this.Weights[i], centroid), 2);

                        for (int j = 0; j < this.indexes.Count; j++)
                        {
                            double cen = getNearestCentroid(j);
                            denominator += Math.Pow(getDistance(this.Weights[j], cen), 2);
                        }
                    }
                    probability = numerator / denominator;
                    if (probability >= maxProbability)
                    {
                        maxProbability = probability;
                        chosenindex = i;
                    }
                }
                Clusters.Add(new List<int>());
                this.Centroids.Add(this.Weights[chosenindex]);

            }

        }

        private double getNearestCentroid(int i)
        {
            double nearestCentroid = double.MaxValue;
            double minimumDistance = double.MaxValue;

            for (int j = 0; j < this.Centroids.Count; j++)
            {
                double distance = getDistance(this.Weights[i], this.Centroids[j]);
                if (distance < minimumDistance)
                {
                    minimumDistance = distance;
                    nearestCentroid = this.Centroids[j];
                }
            }
            return nearestCentroid;
        }


        private double getDistance(double VerbFrameWeight, double Centroid)
        {
            return Math.Abs(VerbFrameWeight - Centroid);
        }

        public List<double> getCentroids()
        {
            List<double> newCentroids = new List<double>();
            for (int i = 0; i < this.Clusters.Count; i++)
            {
                double sum = 0;
                foreach (int index in this.Clusters[i])
                {
                    sum += this.Weights[index];
                }
                double count = this.Clusters[i].Count;
                newCentroids.Add(sum / count);
            }
            return newCentroids;
        }

        private void clearClusters()
        {
            for (int i = 0; i < this.k; i++)
                Clusters[i].Clear();
        }

        private bool centroidsChanged(List<double> NewCentroids)
        {
            bool result = false;

            for (int i = 0; i < this.k; i++)
            {
                if (Math.Round(NewCentroids[i], 5) != Math.Round(this.Centroids[i], 5))
                {
                    result = true;
                    this.Centroids[i] = NewCentroids[i];
                }
            }

            return result;
        }

        public double GetAverageVariance()
        {
            double totalSum = 0;
            double sum = 0;
            for (int i = 0; i < this.k; i++)
            {
                sum = 0;
                foreach (int index in this.Clusters[i])
                {
                    sum += Math.Pow(getDistance(this.Weights[index], this.Centroids[i]), 2);
                }
                sum = sum / this.Clusters[i].Count;
                totalSum += sum;
            }
            totalSum = totalSum / this.k;
            return totalSum;
        }

        
    }
}
