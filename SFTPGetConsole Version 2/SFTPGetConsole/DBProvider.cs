using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFTPGetConsole
{
    public class DBProvider
    {
        #region--Instance--
        public static DBProvider Instance = new DBProvider();
        #endregion

        #region--Download Parameters--
        public List<SFTPmap> DownloadParameters()
        {
            List<SFTPmap> rType = new List<SFTPmap>();
            using (IBSEntities db = new IBSEntities())
            {
                rType = db.SFTPmaps.ToList();
            }
            return rType;
        }
        #endregion
    }
}
