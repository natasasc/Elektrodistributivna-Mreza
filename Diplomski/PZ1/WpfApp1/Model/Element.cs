using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    public class Element
    {
        int x;
        int y;
        bool free;
        long id;
        bool lineX;     // postoji linija paralelna sa X
        bool lineY;     // postoji linija paralelna sa Y
        bool visited;
        int parentX;
        int parentY;

        public Element() 
        { 
            visited = false; 
            parentX = -1;
            parentY = -1;
        }

        public Element(int x, int y, long id)
        {
            this.x = x;
            this.y = y;
            free = true;
            this.id = id;
            lineX = false;
            lineY = false;
            visited = false;
            parentX = -1;
            parentY = -1;
        }

        public Element(int x, int y, bool free, long id)
        {
            this.x = x;
            this.y = y;
            this.free = free;
            this.id = id;
            lineX = false;
            lineY = false;
            visited = false;
            parentX = -1;
            parentY = -1;
        }

        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public bool Free
        {
            get
            {
                return free;
            }
            set
            {
                free = value;
            }
        }

        public long Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public bool LineX
        {
            get
            {
                return lineX;
            }
            set
            {
                lineX = value;
            }
        }

        public bool LineY
        {
            get
            {
                return lineY;
            }
            set
            {
                lineY = value;
            }
        }

        public bool Visited 
        {
            get
            {
                return visited;
            }
            set
            {
                visited = value;
            } 
        }

        public int ParentX
        {
            get
            {
                return parentX;
            }
            set
            {
                parentX = value;
            }
        }

        public int ParentY
        {
            get
            {
                return parentY;
            }
            set
            {
                parentY = value;
            }
        }
    }
}
