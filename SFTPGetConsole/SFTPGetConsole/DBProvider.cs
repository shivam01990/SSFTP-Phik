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
        public List<vw_importSFTP> DownloadParameters()
        {
            List<vw_importSFTP> rType = new List<vw_importSFTP>();
            using (IBSEntities db = new IBSEntities())
            {
                rType = db.vw_importSFTP.ToList();
            }
            return rType;
        }
        #endregion
    }
}
