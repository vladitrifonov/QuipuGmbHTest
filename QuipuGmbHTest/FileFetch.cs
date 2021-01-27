using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuipuGmbHTest
{
    public class FileFetch : IFetch
    {
        private readonly string filename;
        public FileFetch(string filename)
        {
            this.filename = filename;
        }
        public List<string> Fetch()
        {
            return File.ReadAllLines(filename).ToList();           
        }
    }
}
