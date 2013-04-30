using System;
using System.Collections.Generic;

namespace bombersquad_ai
{
	namespace astar
	{
		public class AStar
		{
			
			//private static readonly int PRIORITY_QUEUE_INIT_SIZE = 100;
	
			/**
	 * 
	 * @param graph
	 * @param start a node with valid coords
	 * @param goal a node with valid coords
	 * @param heuristic an admissible heuristic
	 * @return null if no path found, otherwise the path
	 */
			public List<Connection> pathfindAStar (Graph graph, Node start, Node goal, Heuristic heuristic)
			{
		
				//long totalClosedListIterationTime = 0;
				//long totalOpenListIterationTime = 0;
				//long heuristicTime = 0;
				//long openAddTime = 0;
				//long openRemoveTime = 0;
				//long closedAddTime = 0;
				//long closedRemoveTime = 0;
				//long containTime = 0;
		
				int openCount = 0; //keep track of how many open nodes are visited
		
				SearchRecord startRecord = new SearchRecord (start);
				startRecord.setEstimatedTotalCost (heuristic.estimate (start));
		
				//SmallestEstimatedCostComparator comparator = new SmallestEstimatedCostComparator ();
				//PriorityQueue<SearchRecord> openList = new PriorityQueue<SearchRecord> (PRIORITY_QUEUE_INIT_SIZE, comparator);
				List<SearchRecord> openList = new List<SearchRecord>();
				int numNodes = graph.getNumNodes ();
				SearchRecord[] openListLookup = new SearchRecord[numNodes];
		
				SearchRecord[] closedListLookup = new SearchRecord[numNodes];

				openList.Add (startRecord);
				openCount++;
				openListLookup [startRecord.GetHashCode()] = startRecord;

				SearchRecord current = null;
				while (openList.Count > 0) {
					openList.Sort();
					SearchRecord[] openListRecords = openList.ToArray();
					current = openListRecords[0]; //retrieve, but do not remove
					//Console.WriteLine("current node: " + current);
					//current = openList.peek (); //retrieve, but do not remove
			
					if (current.getNode ().Equals (goal)) {
						break;
					}
			
					List<Connection> connections = graph.getConnections (current.getNode ());
			
					foreach (Connection conn in connections) {
						
						//Console.WriteLine("checking connection " + conn);
						Node endNode = conn.getToNode ();
						double endNodeCost = current.getCostSoFar () + conn.getCost ();
						SearchRecord endNodeRecord = null;
						double endNodeHeuristic = 0.0;
						//create temporary SearchRecord to wrap endNode... for purposes of searching the open and closed lists
						SearchRecord endNodeRecordTemplate = new SearchRecord (endNode);
				
						//long containStart = System.currentTimeMillis ();
						bool closedListContains = (closedListLookup [endNodeRecordTemplate.GetHashCode()] != null);
						bool openListContains = false;
						//only need to determine openListContains value if closedListContains is false (due to the if/else ordering below)
						if (!(closedListContains)) {
							openListContains = (openListLookup [endNodeRecordTemplate.GetHashCode()] != null);
						}
						//long containEnd = System.currentTimeMillis ();
						//containTime += (containEnd - containStart);
				
						if (closedListContains) {
					
							//find end node record from closed list
							//long closedListIterationStart = System.currentTimeMillis ();
							endNodeRecord = closedListLookup [endNodeRecordTemplate.GetHashCode()];
							//long closedListIterationEnd = System.currentTimeMillis ();
							//totalClosedListIterationTime += (closedListIterationEnd - closedListIterationStart);
					
							if (endNodeRecord.getCostSoFar () <= endNodeCost) {
								continue;
							}
					
					
							//long closedRemoveStart = System.currentTimeMillis ();
							closedListLookup [endNodeRecord.GetHashCode()] = null;
							//long closedRemoveEnd = System.currentTimeMillis ();
							//closedRemoveTime += closedRemoveEnd - closedRemoveStart;
							endNodeHeuristic = endNodeRecord.getEstimatedTotalCost () - endNodeRecord.getCostSoFar ();
					
						} else if (openListContains) {
					
							//find end node record from open list
							//long openListIterationStart = System.currentTimeMillis ();
							endNodeRecord = openListLookup [endNodeRecordTemplate.GetHashCode()];
							//long openListIterationEnd = System.currentTimeMillis ();
							//totalOpenListIterationTime += (openListIterationEnd - openListIterationStart);
					
							if (endNodeRecord.getCostSoFar () <= endNodeCost) {
								continue;
							}
					
							endNodeHeuristic = endNodeRecord.getEstimatedTotalCost () - endNodeRecord.getCostSoFar ();
						} else {
							//unvisited node
							endNodeRecord = new SearchRecord (endNode);
							//long heuristicStartTime = System.currentTimeMillis ();
							endNodeHeuristic = heuristic.estimate (endNode);
							//long heuristicEndTime = System.currentTimeMillis ();
							//heuristicTime += (heuristicEndTime - heuristicStartTime);
						}
				
						//update the cost, estimate, and connection
						endNodeRecord.setCostSoFar (endNodeCost);
						endNodeRecord.setConnection (conn);
						//Console.WriteLine("record " + endNodeRecord + ", setting connection: " + conn);
						endNodeRecord.setEstimatedTotalCost (endNodeCost + endNodeHeuristic);
				
						//long containsCheckStart = System.currentTimeMillis ();
						bool openListContainsEndNodeRecord = (openListLookup [endNodeRecord.GetHashCode()] != null);
						//long containsCheckEnd = System.currentTimeMillis ();
						//containTime += (containsCheckEnd - containsCheckStart);
				
						if (!(openListContainsEndNodeRecord)) {
							//long openAddStart = System.currentTimeMillis ();
							openList.Add (endNodeRecord);
							//Console.WriteLine("Adding " + endNodeRecord + " to open list");
							openList.Sort();
							openCount++;
							openListLookup [endNodeRecord.GetHashCode()] = endNodeRecord;
							//long openAddEnd = System.currentTimeMillis ();
							//openAddTime += (openAddEnd - openAddStart);
						}
					}
			
					//long openRemoveStart = System.currentTimeMillis ();
					//openList.Sort();
					openList.Remove(current); //RemoveAt(0);
					//openList.poll (); //NOTE: current is always the head of the open list... old code: //openList.remove(current);
					openListLookup [current.GetHashCode()] = null;
					//long openRemoveEnd = System.currentTimeMillis ();
					//openRemoveTime += (openRemoveEnd - openRemoveStart);
					//long closedAddStart = System.currentTimeMillis ();
					closedListLookup [current.GetHashCode()] = current;
					//long closedAddEnd = System.currentTimeMillis ();
					//closedAddTime += (closedAddEnd - closedAddStart);
			
				}
		
				if (!(current.getNode ().Equals (goal))) {
					return null;
				} else {
					List<Connection> reversePath = new List<Connection> ();
			
					//retrieve path
					while (!(current.getNode().Equals(start))) {
						//Console.WriteLine("adding to reversePath: " + current.getConnection());
						reversePath.Add (current.getConnection ());
						Node fromNode = current.getConnection ().getFromNode ();
						SearchRecord placeholder = new SearchRecord (fromNode);
						SearchRecord previous = closedListLookup [placeholder.GetHashCode()];
						if (previous == null) {
							String to = (placeholder.getConnection ().getToNode () != null) ? placeholder.getConnection ().getToNode ().ToString () : "null";
							String from = (placeholder.getConnection ().getFromNode () != null) ? placeholder.getConnection ().getFromNode ().ToString () : "null";
							//Console.WriteLine ("retrieving path NPE. Data: Placeholder connection from: " + from + ", placeholder connection to: " + to + ", goal: " + goal + ", start: " + start);
						}
						//find SearchRecord for fromNode in the closed list
						current = previous;
				
					}
			
					//reverse the path
					List<Connection> path = new List<Connection> ();
					//Enumerator enumerator = reversePath.GetEnumerator();
					//Iterator<Connection> reverseIt = reversePath.iterator ();
					//while (enumerator.hasNext()) {
					foreach (Connection reverseConn in reversePath) {
						//Console.WriteLine("reverseConn " + reverseConn);
						//Connection reverseConn = reverseIt.next ();
						//path.addFirst (reverseConn);
						path.Insert(0, reverseConn);
					}
			
//			System.out.println("total closed it time: " + totalClosedListIterationTime);
//			System.out.println("total open it time: " + totalOpenListIterationTime);
//			System.out.println("heuristic time: " + heuristicTime);
//			System.out.println("open add time: " + openAddTime);
//			System.out.println("open remove time: " + openRemoveTime);
//			System.out.println("closed add time: " + closedAddTime);
//			System.out.println("contain time: " + containTime);
//			System.out.println("closed remove time: " + closedRemoveTime);
//			long sum = totalClosedListIterationTime + totalOpenListIterationTime + heuristicTime + openAddTime + openRemoveTime + closedAddTime + containTime + closedRemoveTime;
//			System.out.println("total accounted time: " + sum);
//			System.out.println("open nodes visited: " + openCount);
			
					return path;
				}
		
			}
	
	
		}
		
		/*
		class SmallestEstimatedCostComparator : System.Collections.IComparer<SearchRecord>
		{

			//@Override
			int IComparer.Compare (SearchRecord r1, SearchRecord r2)
			{
				if (r1.getEstimatedTotalCost () < r2.getEstimatedTotalCost ()) {
					return -1;
				} else if (r1.getEstimatedTotalCost () == r2.getEstimatedTotalCost ()) {
					return 0;
				} else
					return 1;
			}
		
		}
		*/
	
		class SearchRecord : IComparable
		{
			private Node node;
			private Connection connection;
			private double costSoFar;
			private double estimatedTotalCost;
		
			public SearchRecord (Node node)
			{
				this.node = node;
				this.connection = null;
				this.costSoFar = 0;
				this.estimatedTotalCost = 0;
			}

			public Node getNode ()
			{
				return node;
			}

			public double getCostSoFar ()
			{
				return this.costSoFar;
			}
		
			public void setCostSoFar (double costSoFar)
			{
				this.costSoFar = costSoFar;
			}

			public Connection getConnection ()
			{
				return connection;
			}

			public void setConnection (Connection connection)
			{
				this.connection = connection;
			}

			public double getEstimatedTotalCost ()
			{
				return estimatedTotalCost;
			}

			public void setEstimatedTotalCost (double estimatedTotalCost)
			{
				this.estimatedTotalCost = estimatedTotalCost;
			}
		
			public override bool Equals (Object obj)
			{
				bool result = (obj is SearchRecord);
				if (!result) {
					return false;
				}
				//if (!(obj instanceof SearchRecord)) { return false; }
				SearchRecord other = (SearchRecord)obj;
				return this.node.Equals (other.node);
			}

			public override int GetHashCode ()
			{
				return this.node.GetHashCode ();
			}
			
			public int CompareTo(object obj)
      		{
				SearchRecord other = (SearchRecord)obj;
				//Console.WriteLine("comparing searchrecord " + this.getEstimatedTotalCost() + " to searchrecord " + other.getEstimatedTotalCost());
				if (this.getEstimatedTotalCost () < other.getEstimatedTotalCost ()) {
					return -1;
				} else if (this.getEstimatedTotalCost () == other.getEstimatedTotalCost ()) {
					return 0;
				} else
					return 1;
			}
			
			public override string ToString ()
			{
				MapLocation ml = (MapLocation)this.node;
				string theString = "Searchrecord for node " + ml + " with connection " + this.connection;
				return theString;
			}
		}
	}
}