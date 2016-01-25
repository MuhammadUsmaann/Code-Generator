using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CodeGenerator.MVP.Util
{
    public interface IObjectBuilder
    {
        StringBuilder BuildObject(string strProjectName, string strObjectName, DataTable dt);
        StringBuilder BuildEntity(string strProjectName, string strObjectName, DataTable dt, String InterfaceClass);
        StringBuilder BuildSevice(string strProjectName, string strObjectName, DataTable dt, String InterfaceClass);
        StringBuilder BuildDAL(string strProjectName, string strObjectName, DataTable dt);
        StringBuilder BuildDATA(string strProjectName, string strObjectName, DataTable dt);
        StringBuilder BuildPRESENTER(string strProjectName, string strObjectName, DataTable dt, out StringBuilder sbView);
    }
}
