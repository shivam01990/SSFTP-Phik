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
        public List<vw_importDownloadParameters> DownloadParameters()
        {
            List<vw_importDownloadParameters> rType = new List<vw_importDownloadParameters>();
            using (IBSEntities db = new IBSEntities())
            {
                rType = db.vw_importDownloadParameters.ToList();
            }
            return rType;
        }
        #endregion
    }
}
