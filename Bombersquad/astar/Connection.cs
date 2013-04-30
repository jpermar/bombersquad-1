using System;

namespace bombersquad_ai
{
	namespace astar
	{
		public class Connection
		{
			private Node from;
			private Node to;
			private double cost;
	
			public Connection (Node from, Node to, double cost)
			{
				this.from = from;
				this.to = to;
				this.cost = cost;
			}
	
			public double getCost ()
			{
				return this.cost;
			}
	
			public Node getFromNode ()
			{
				return this.from;
			}
	
			public Node getToNode ()
			{
				return this.to;
			}
			
			public override string ToString ()
			{
				return "(" + this.from + ", " + this.to;
			}
		}
	}
}

