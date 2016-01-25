using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CodeGenerator.MVP.Util;

namespace CodeGenerator.MVP.Util
{
    public class ObjectBuilderBase : IObjectBuilder
    {
        protected string _NUMID = "";
        protected string _STRUID = "";
        protected string _STRLASTUID = "";
        protected string _DTUDT = "";
        protected string _DTLASTUDT = "";

        public ObjectBuilderBase()
        {
            _NUMID = Util.Utility.GetPkColName();
            _STRUID = Util.Utility.GetRecordCreatorColName();
            _STRLASTUID = Util.Utility.GetRecordModifierColName();
            _DTUDT = Util.Utility.GetRecordCreateDateColName();
            _DTLASTUDT = Util.Utility.GetRecordModifiedDateColName();
        }

        public virtual StringBuilder BuildObject(string strProjectName, string strObjectName, DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(" using System;");
            sb.Append("\n using System.Collections.Generic;");
            sb.Append("\n using System.Linq;");
            sb.Append("\n using System.Text;");
            sb.Append("\n using System.Data;");
            sb.Append("\n using System.Runtime.Serialization;");
            sb.Append("\n using System.Globalization;");

            sb.Append("\n\n namespace " + strProjectName + ".DAL.DTO");
            sb.Append("\n {");
            sb.Append("\n\t[Serializable]");
            sb.Append("\n\tpublic class " + strObjectName + " : EntityBase");
            sb.Append("\n\t{");

            sb.Append("\n\t\tpublic " + strObjectName + "() { }");

            foreach (DataRow row in dt.Rows)
            {
                if (row["COLUMN_NAME"].ToString().Contains(_STRUID) || row["COLUMN_NAME"].ToString().Contains(_STRLASTUID) ||
                    row["COLUMN_NAME"].ToString().Contains(_DTUDT) || row["COLUMN_NAME"].ToString().Contains(_DTLASTUDT))
                    continue;

                sb.AppendLine();
                sb.Append("\n\t\t[DataMember, DataColumn(true)]");

                string strDataType = Utility.GetDotNetDataType(row["DATA_TYPE"].ToString().ToUpper(), row["PRECISION"].ToString(), row["SCALE"].ToString());
                sb.Append("\n\t\tpublic " + strDataType + " " + row["COLUMN_NAME"] + "  { get; set; }");
            }

            sb.Append("\n\t}");
            sb.Append("\n }");

            return sb;
        }

        public virtual StringBuilder BuildDAL(string strProjectName, string strObjectName, DataTable dt)
        {
            return new StringBuilder();
        }

        public virtual StringBuilder BuildDATA(string strProjectName, string strObjectName, DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(" using System;");
            sb.Append("\n using System.Data;");
            sb.Append("\n using System.Collections.Generic;");
            sb.Append("\n using System.Linq;");
            sb.Append("\n using System.Text;");
            sb.Append("\n using " + strProjectName + ".DAL;");
            sb.Append("\n using " + strProjectName + ".DAL.DTO;");
            sb.AppendLine();

            sb.Append("\n namespace " + strProjectName + ".Data");
            sb.Append("\n {");

            sb.Append("\n\tpublic class " + strObjectName + "Data");
            sb.Append("\n\t{");

            #region SaveObj
            if (Utility.GetSelectedDB().Equals("Oracle"))
            {
                sb.AppendLine();
                sb.Append("\n\t\tpublic int Save(" + strObjectName + " obj, DBTransaction transaction)");
                sb.Append("\n\t\t{");
                sb.Append("\n\t\t\tint i = -1;");
                sb.Append("\n\t\t\ti = " + strObjectName + "DAL.SaveObj(obj, \"I\", transaction);");
                sb.Append("\n\t\t\treturn i;");
                sb.Append("\n\t\t}");
                sb.AppendLine();
                sb.Append("\n\t\tpublic int Update(" + strObjectName + " obj, DBTransaction transaction)");
                sb.Append("\n\t\t{");
                sb.Append("\n\t\t\tint i = -1;");
                sb.Append("\n\t\t\ti = " + strObjectName + "DAL.SaveObj(obj, \"U\", transaction);");
                sb.Append("\n\t\t\treturn i;");
                sb.Append("\n\t\t}");
                sb.AppendLine();
                sb.Append("\n\t\tpublic int Delete(" + strObjectName + " obj, DBTransaction transaction)");
                sb.Append("\n\t\t{");
                sb.Append("\n\t\t\tint i = -1;");
                sb.Append("\n\t\t\ti = " + strObjectName + "DAL.SaveObj(obj, \"D\", transaction);");
                sb.Append("\n\t\t\treturn i;");
                sb.Append("\n\t\t}");
            }
            else if (Util.Utility.GetSelectedDB().Equals("SqlServer"))
            {
                sb.AppendLine();
                sb.Append("\n\t\tpublic string Save(" + strObjectName + " obj, DBTransaction transaction, out int intErrorCode)");
                sb.Append("\n\t\t{");
                sb.Append("\n\t\t\tstring uid = \"\";");
                sb.Append("\n\t\t\tuid = " + strObjectName + "DAL.SaveObj(obj, \"I\", transaction, out intErrorCode);");
                sb.Append("\n\t\t\treturn uid;");
                sb.Append("\n\t\t}");
                sb.AppendLine();
                sb.Append("\n\t\tpublic string Update(" + strObjectName + " obj, DBTransaction transaction, out int intErrorCode)");
                sb.Append("\n\t\t{");
                sb.Append("\n\t\t\tstring uid = \"\";");
                sb.Append("\n\t\t\tuid = " + strObjectName + "DAL.SaveObj(obj, \"U\", transaction, out intErrorCode);");
                sb.Append("\n\t\t\treturn uid;");
                sb.Append("\n\t\t}");
                sb.AppendLine();
                sb.Append("\n\t\tpublic string Delete(" + strObjectName + " obj, DBTransaction transaction, out int intErrorCode)");
                sb.Append("\n\t\t{");
                sb.Append("\n\t\t\tstring uid = \"\";");
                sb.Append("\n\t\t\tuid = " + strObjectName + "DAL.SaveObj(obj, \"D\", transaction, out intErrorCode);");
                sb.Append("\n\t\t\treturn uid;");
                sb.Append("\n\t\t}");
            }
            #endregion //End of SaveObj

            #region GetObjCount
            sb.AppendLine();
            sb.Append("\n\t\tpublic int GetObjCount(");
            //Paremeters
            string strPatrams = "";
            string strPatramsWithoutDataType = "";
            foreach (DataRow row in dt.Rows)
            {
                if (row["COLUMN_NAME"].ToString().Contains(_STRUID) || row["COLUMN_NAME"].ToString().Contains(_STRLASTUID) ||
                    row["COLUMN_NAME"].ToString().Contains(_DTUDT) || row["COLUMN_NAME"].ToString().Contains(_DTLASTUDT))
                    continue;

                string strDataType = Utility.GetDotNetDataType(row["DATA_TYPE"].ToString().ToUpper(), row["PRECISION"].ToString(), row["SCALE"].ToString());
                strPatrams += " " + strDataType + " " + row["COLUMN_NAME"] + ",";
                strPatramsWithoutDataType += " " + row["COLUMN_NAME"] + ",";
            }
            strPatrams = strPatrams.TrimEnd(',');
            strPatramsWithoutDataType = strPatramsWithoutDataType.TrimEnd(',');
            sb.Append(strPatrams + ")");
            sb.Append("\n\t\t{");
            sb.Append("\n\t\t\treturn " + strObjectName + "DAL.GetObjCount(" + strPatramsWithoutDataType + ");");
            sb.Append("\n\t\t}");
            #endregion //End of GetObjCount

            #region GetObjList
            sb.AppendLine();
            sb.Append("\n\t\tpublic List<" + strObjectName + "> GetObjList(");
            sb.Append(strPatrams + ", \n\t\t\t\t\t\t" + Util.Utility.GetCommonSearchParams(true)+ ")");
            
            sb.Append("\n\t\t{");
            sb.Append("\n\t\t\treturn " + strObjectName + "DAL.GetObjList(");
            sb.Append(strPatramsWithoutDataType + ", \n\t\t\t\t\t\t" + Util.Utility.GetCommonSearchParams(false) + ");");

            sb.Append("\n\t\t}");
            #endregion //End of GetObjList

            sb.Append("\n\t}");
            sb.Append("\n }");

            return sb;
        }

        public virtual StringBuilder BuildPRESENTER(string strProjectName, string strObjectName, DataTable dt, out StringBuilder sbView)
        {
            StringBuilder sb = new StringBuilder();

            #region IViewInterface
            sbView = new StringBuilder();
            sbView.Append(" using System;");
            sbView.Append("\n using System.Collections.Generic;");
            sbView.Append("\n using System.Linq;");
            sbView.Append("\n using System.Text;");

            sbView.Append("\n\n namespace " + strProjectName + ".Presenter");
            sbView.Append("\n {");
            sbView.Append("\n\tpublic interface I" + strObjectName + "View");
            sbView.Append("\n\t{");

            foreach (DataRow row in dt.Rows)
            {
                if (row["COLUMN_NAME"].ToString().Contains(_STRUID) || row["COLUMN_NAME"].ToString().Contains(_STRLASTUID) ||
                    row["COLUMN_NAME"].ToString().Contains(_DTUDT) || row["COLUMN_NAME"].ToString().Contains(_DTLASTUDT))
                    continue;

                //sbView.AppendLine();
                string strDataType = Utility.GetDotNetDataType(row["DATA_TYPE"].ToString().ToUpper(), row["PRECISION"].ToString(), row["SCALE"].ToString());
                sbView.Append("\n\t\t" + strDataType + " " + row["COLUMN_NAME"] + "  { get; set; }");
            }
            sbView.Append("\n\t\tint CanEdit  { get; set; }");
            sbView.Append("\n\t\tstring ErrorMessage  { get; set; }");
            sbView.Append("\n\t}"); //end of I.View

            sbView.AppendLine();
            sbView.AppendLine();
            sbView.Append("\n\tpublic interface I" + strObjectName + "SearchView");
            sbView.Append("\n\t{");

            foreach (DataRow row in dt.Rows)
            {
                if (row["COLUMN_NAME"].ToString().Contains(_STRUID) || row["COLUMN_NAME"].ToString().Contains(_STRLASTUID) ||
                    row["COLUMN_NAME"].ToString().Contains(_DTUDT) || row["COLUMN_NAME"].ToString().Contains(_DTLASTUDT))
                    continue;

                //sbView.AppendLine();
                string strDataType = Utility.GetDotNetDataType(row["DATA_TYPE"].ToString().ToUpper(), row["PRECISION"].ToString(), row["SCALE"].ToString());
                sbView.Append("\n\t\t" + strDataType + " " + row["COLUMN_NAME"] + "  { get; }");
            }
            sbView.Append("\n\t}"); //end of I.SearchView
            sbView.Append("\n }"); //end of Namespace
            #endregion end of IView

            #region Presenter

            sb.Append(" using System;");
            sb.Append("\n using System.Data;");
            sb.Append("\n using System.Collections.Generic;");
            sb.Append("\n using System.Linq;");
            sb.Append("\n using System.Text;");
            sb.Append("\n using " + strProjectName + ".Data;");
            sb.Append("\n using " + strProjectName + ".DAL.DTO;");
            sb.AppendLine();

            sb.Append("\n namespace " + strProjectName + ".Presenter");
            sb.Append("\n {");

            sb.Append("\n\tpublic class " + strObjectName + "Presenter");
            sb.Append("\n\t{");
            sb.Append("\n\t\tprivate I" + strObjectName + "View IView;");
            sb.Append("\n\t\tprivate " + strObjectName + "Data Data;");

            sb.AppendLine();
            sb.Append("\n\t\tpublic " + strObjectName + "Presenter(I" + strObjectName + "View view)");
            sb.Append("\n\t\t{");
            sb.Append("\n\t\t\tthis.IView = view;");
            sb.Append("\n\t\t\tData = new " + strObjectName + "Data();");
            sb.Append("\n\t\t}");

            #region PopulateObj
            sb.AppendLine();
            sb.Append("\n\t\tpublic void PopulateObj()");
            sb.Append("\n\t\t{");
            sb.Append("\n\t\t\t" + strObjectName + " obj = new " + strObjectName + "();");
            sb.AppendLine();
            sb.Append("\n\t\t\tif (IView." + _NUMID + " != null)");
            sb.Append("\n\t\t\t{");

            string strPatramsDefaultValues = "";
            foreach (DataRow row in dt.Rows)
            {
                if (row["COLUMN_NAME"].ToString().Contains(_STRUID) || row["COLUMN_NAME"].ToString().Contains(_STRLASTUID) ||
                    row["COLUMN_NAME"].ToString().Contains(_DTUDT) || row["COLUMN_NAME"].ToString().Contains(_DTLASTUDT))
                    continue;

                string strDataType = Utility.GetDotNetDataType(row["DATA_TYPE"].ToString().ToUpper(), row["PRECISION"].ToString(), row["SCALE"].ToString());
                strPatramsDefaultValues += ", " + Util.Utility.GetDotNetDataTypeDefaultValue(strDataType);
            }
            sb.Append("\n\t\t\t\tobj = Data.GetObjList(IView." + _NUMID + strPatramsDefaultValues + " 2, \"T." + _NUMID + "\", \"ASC\", 1, 1).SingleOrDefault();");

            sb.AppendLine();
            sb.Append("\n\t\t\t\tif (obj != null)");
            sb.Append("\n\t\t\t\t{");
            //Paremeters
            string strGetPatrams = "";
            string strSavePatrams = "";
            foreach (DataRow row in dt.Rows)
            {
                /*if (row["COLUMN_NAME"].ToString().Contains(_STRUID) || row["COLUMN_NAME"].ToString().Contains(_STRLASTUID) ||
                    row["COLUMN_NAME"].ToString().Contains(_DTUDT) || row["COLUMN_NAME"].ToString().Contains(_DTLASTUDT))
                    continue;
                string strDataType = Utility.GetDotNetDataType(row["DATA_TYPE"].ToString().ToUpper());*/
                strGetPatrams += "\n\t\t\t\t\t" + "IView." + row["COLUMN_NAME"] + " = " + "obj." + row["COLUMN_NAME"] + ";";
                strSavePatrams += "\n\t\t\t\t" + "obj." + row["COLUMN_NAME"] + " = " + "IView." + row["COLUMN_NAME"] + ";";
            }
            sb.Append(strGetPatrams);
            sb.Append("\n\t\t\t\t\t" + "IView.CanEdit = obj.CanEdit;");
            sb.Append("\n\t\t\t\t}");
            sb.Append("\n\t\t\t}");
            sb.Append("\n\t\t}");
            #endregion

            #region SaveObj
            sb.AppendLine();
            if (Util.Utility.GetSelectedDB().Equals("Oracle"))
            {
                sb.Append("\n\t\tpublic int SaveObj()");
                sb.Append("\n\t\t{");
                sb.Append("\n\t\t\tint i = -1;");
            }
            else if (Util.Utility.GetSelectedDB().Equals("SqlServer"))
            {
                sb.Append("\n\t\tpublic string SaveObj(out int intErrorCode)");
                sb.Append("\n\t\t{");
                sb.Append("\n\t\t\tintErrorCode = 0;");
                sb.Append("\n\t\t\tstring uid = \"\";");
            }

            sb.Append("\n\t\t\t" + strObjectName + " obj = new " + strObjectName + "();");
            sb.Append("\n\t\t\tDBTransaction transaction = new DBTransaction();");

            sb.AppendLine();
            sb.Append("\n\t\t\ttry");
            sb.Append("\n\t\t\t{");
            sb.Append("\n\t\t\t\tif (transaction != null) transaction.Begin();");
            sb.Append(strSavePatrams); //Paremeters

            sb.AppendLine();
            if (Util.Utility.GetSelectedDB().Equals("Oracle"))
            {
                sb.Append("\n\t\t\t\tif (IView.NUMID > 0)");
                sb.Append("\n\t\t\t\t{");
                sb.Append("\n\t\t\t\t\ti = Data.Update(obj, transaction);");
                sb.Append("\n\t\t\t\t}");
                sb.Append("\n\t\t\t\telse");
                sb.Append("\n\t\t\t\t{");
                sb.Append("\n\t\t\t\t\ti = Data.Save(obj, transaction);");
                sb.Append("\n\t\t\t\t}");
                sb.AppendLine();
                sb.Append("\n\t\t\t\tif (i > 0)");
            }
            else if (Util.Utility.GetSelectedDB().Equals("SqlServer"))
            {
                sb.Append("\n\t\t\t\tif (IView.uidPK != \"\")");
                sb.Append("\n\t\t\t\t{");
                sb.Append("\n\t\t\t\t\tuid = Data.Update(obj, dbTransaction, out intErrorCode);");
                sb.Append("\n\t\t\t\t}");
                sb.Append("\n\t\t\t\telse");
                sb.Append("\n\t\t\t\t{");
                sb.Append("\n\t\t\t\t\tuid = Data.Save(obj, dbTransaction, out intErrorCode);");
                sb.Append("\n\t\t\t\t}");
                sb.AppendLine();
                sb.Append("\n\t\t\t\tif (intErrorCode >= 0)");
            }

            //sb.Append("\n\t\t\t\t{");
            sb.Append("\n\t\t\t\t\ttransaction.Commit();");
            //sb.Append("\n\t\t\t\t}");
            sb.Append("\n\t\t\t\telse");
            //sb.Append("\n\t\t\t\t{");
            sb.Append("\n\t\t\t\t\tthrow new Exception();");
            //sb.Append("\n\t\t\t\t}");
            sb.Append("\n\t\t\t}"); //end of Try
            sb.Append("\n\t\t\tcatch (Exception ex)");
            sb.Append("\n\t\t\t{");
            sb.Append("\n\t\t\t\ttransaction.RollBack();");
            sb.Append("\n\t\t\t}");
            sb.Append("\n\t\t\tfinally");
            sb.Append("\n\t\t\t{");
            sb.Append("\n\t\t\t\ttransaction.Dispose();");
            sb.Append("\n\t\t\t}");
            sb.AppendLine();

            if (Util.Utility.GetSelectedDB().Equals("Oracle"))
            {
                sb.Append("\n\t\t\treturn i;");
            }
            else if (Util.Utility.GetSelectedDB().Equals("SqlServer"))
            {
                sb.Append("\n\t\t\treturn uid;");
            }
            sb.Append("\n\t\t}");
            #endregion

            #region DeleteObj
            sb.AppendLine();
            if (Util.Utility.GetSelectedDB().Equals("Oracle"))
            {
                sb.Append("\n\t\tpublic int Delete()");
                sb.Append("\n\t\t{");
                sb.Append("\n\t\t\tint i = -1;");
            }
            else if (Util.Utility.GetSelectedDB().Equals("SqlServer"))
            {
                sb.Append("\n\t\tpublic string Delete(out int intErrorCode)");
                sb.Append("\n\t\t{");
                sb.Append("\n\t\t\tintErrorCode = 0;");
                sb.Append("\n\t\t\tstring uid = \"\";");
            }

            sb.Append("\n\t\t\t" + strObjectName + " obj = new " + strObjectName + "();");
            sb.Append("\n\t\t\tDBTransaction transaction = new DBTransaction();");
            sb.AppendLine();
            sb.Append("\n\t\t\ttry");
            sb.Append("\n\t\t\t{");
            sb.Append("\n\t\t\t\tif (transaction != null) transaction.Begin();");
            sb.AppendLine();
            sb.Append("\n\t\t\t\tobj." + _NUMID + " = IView." + _NUMID + ";");
            sb.Append("\n\t\t\t\tobj." + _STRUID + " = IView." + _STRUID + ";");
            if (Util.Utility.GetSelectedDB().Equals("Oracle"))
            {
                sb.Append("\n\t\t\t\ti = Data.Delete(obj, transaction);");
                sb.AppendLine();
                sb.Append("\n\t\t\t\tif (i >= 0)");
            }
            else if (Util.Utility.GetSelectedDB().Equals("SqlServer"))
            {
                sb.Append("\n\t\t\t\tuid = Data.Delete(obj, out intErrorCode);");
                sb.AppendLine();
                sb.Append("\n\t\t\t\tif (intErrorCode >= 0)");
            }
            sb.Append("\n\t\t\t\t\ttransaction.Commit();");
            sb.Append("\n\t\t\t\telse");
            sb.Append("\n\t\t\t\t\tthrow new Exception();");
            
            sb.Append("\n\t\t\t}"); //end of Try
            sb.Append("\n\t\t\tcatch (Exception ex)");
            sb.Append("\n\t\t\t{");
            sb.Append("\n\t\t\t\ttransaction.RollBack();");
            sb.Append("\n\t\t\t}");
            sb.Append("\n\t\t\tfinally");
            sb.Append("\n\t\t\t{");
            sb.Append("\n\t\t\t\ttransaction.Dispose();");
            sb.Append("\n\t\t\t}");
            sb.AppendLine();
            if (Util.Utility.GetSelectedDB().Equals("Oracle"))
            {
                sb.Append("\n\t\t\treturn i;");
            }
            else if (Util.Utility.GetSelectedDB().Equals("SqlServer"))
            {
                sb.Append("\n\t\t\treturn uid;");
            }
            sb.Append("\n\t\t}");
            #endregion

            sb.Append("\n\t}"); //end of Presenter

            sb.AppendLine();
            sb.AppendLine();

            #region SearchPresenter
            sb.Append("\n\tpublic class " + strObjectName + "SearchPresenter");
            sb.Append("\n\t{");
            sb.Append("\n\t\tprivate I" + strObjectName + "SearchView IView;");
            sb.Append("\n\t\tprivate " + strObjectName + "Data Data;");

            sb.AppendLine();
            sb.Append("\n\t\tpublic " + strObjectName + "SearchPresenter(I" + strObjectName + "SearchView view)");
            sb.Append("\n\t\t{");
            sb.Append("\n\t\t\tthis.IView = view;");
            sb.Append("\n\t\t\tData = new " + strObjectName + "Data();");
            sb.Append("\n\t\t}");

            sb.AppendLine();
            sb.Append("\n\t\tpublic int GetObjCount(");
            //Paremeters
            string strPatrams = "";
            string strPatramsWithoutDataType = "";
            foreach (DataRow row in dt.Rows)
            {
                if (row["COLUMN_NAME"].ToString().Contains(_STRUID) || row["COLUMN_NAME"].ToString().Contains(_STRLASTUID) ||
                    row["COLUMN_NAME"].ToString().Contains(_DTUDT) || row["COLUMN_NAME"].ToString().Contains(_DTLASTUDT))
                    continue;

                string strDataType = Utility.GetDotNetDataType(row["DATA_TYPE"].ToString().ToUpper(), row["PRECISION"].ToString(), row["SCALE"].ToString());
                strPatrams += " " + strDataType + " " + row["COLUMN_NAME"] + ",";
                strPatramsWithoutDataType += " " + row["COLUMN_NAME"] + ",";
            }
            strPatrams = strPatrams.TrimEnd(',');
            strPatramsWithoutDataType = strPatramsWithoutDataType.TrimEnd(',');
            sb.Append(strPatrams + ")");
            sb.Append("\n\t\t{");
            sb.Append("\n\t\t\treturn Data.GetObjCount(" + strPatramsWithoutDataType + ");");
            sb.Append("\n\t\t}");

            sb.AppendLine();
            sb.Append("\n\t\tpublic List<" + strObjectName + "> GetObjList(");
            sb.Append(strPatrams + ", \n\t\t\t\t\t\t"+ Util.Utility.GetCommonSearchParams(true) +")");
            sb.Append("\n\t\t{");
            sb.Append("\n\t\t\treturn Data.GetObjList(");
            sb.Append(strPatramsWithoutDataType + ", \n\t\t\t\t\t\t" + Util.Utility.GetCommonSearchParams(false) + "" + ");");
            sb.Append("\n\t\t}");
            sb.Append("\n\t}"); //end of SearchPresenter
            #endregion //End of SearchPresenter

            sb.Append("\n }"); //end of Namespace
            #endregion End of Presenter

            return sb;
        }


        public StringBuilder BuildEntity(string strProjectName, string strObjectName, DataTable dt, string InterfaceClass)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(" using System;");
            
            sb.Append("\n\n namespace " + strProjectName);
            sb.Append("\n {");
            sb.Append("\n\tpublic partial class " + strObjectName + " : " + InterfaceClass);
            sb.Append("\n\t{");

            StringBuilder privateProperties = new StringBuilder();
            StringBuilder publicProperties = new StringBuilder();

            var sd = dt.Rows;

            
            foreach (DataRow row in dt.Rows)
            {
                if (row["COLUMN_NAME"].ToString().Contains(_STRUID) || row["COLUMN_NAME"].ToString().Contains(_STRLASTUID) ||
                    row["COLUMN_NAME"].ToString().Contains(_DTUDT) || row["COLUMN_NAME"].ToString().Contains(_DTLASTUDT))
                    continue;

                string strDataType = Utility.GetDotNetDataType(row["DATA_TYPE"].ToString().ToUpper(), row["PRECISION"].ToString(), row["SCALE"].ToString());

                string temp_private_property = (" m_" + row["COLUMN_NAME"]).ToLower();

                if (strDataType.ToLower() == "datetime" || strDataType.ToLower() == "date")
                {
                    strDataType = "string";
                }

                privateProperties.Append("\n\t\tprivate " + strDataType + temp_private_property + ";");

                

                publicProperties.Append("\n\t\tpublic " + strDataType + " " + row["COLUMN_NAME"]);
                publicProperties.Append("\n\t\t{");

                if (row["COLUMN_NAME"].ToString() == "creation_date" || row["COLUMN_NAME"].ToString() == "updated_date")
                {
                    publicProperties.Append("\n\t\t\tget { return GetCurrentDate(); } ");
                }    
                else
                {
                    publicProperties.Append("\n\t\t\tget { return " + temp_private_property + "; } ");
                }

                //publicProperties.Append("\n\t\t\tget { return " + temp_private_property + "; } ");
                publicProperties.Append("\n\t\t\tset{" + temp_private_property + " = value; NotifyPropertyChanged(\"" + row["COLUMN_NAME"] + "\"); }");
                publicProperties.Append("\n\t\t}");
                publicProperties.AppendLine();
            }

            sb.Append(privateProperties.ToString());
            sb.AppendLine();
            sb.Append(publicProperties.ToString());
            sb.Append("\n\t}");
            sb.Append("\n}");

            return sb;
        }


        public StringBuilder BuildSevice(string strProjectName, string strObjectName, DataTable dt, string InterfaceClass)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("\nusing ES.Entities;");
            sb.Append("\nusing System.Collections.Generic;");
            sb.Append("\nusing System.Linq;");

            sb.Append("\n\nnamespace ES.BusinessLayer");
            sb.Append("\n{");
            sb.Append("\n\tpublic partial class " + strObjectName + "Repository : " + InterfaceClass + "<" + strObjectName + ">");
            sb.Append("\n\t{");

            StringBuilder insertCol =new StringBuilder();
            StringBuilder insertValue = new StringBuilder();
            StringBuilder UpdateCols = new StringBuilder();
            string whereCon = "";
            string primaryKey = "";

            foreach (DataRow row in dt.Rows)
            {
                if (row["COLUMN_NAME"].ToString().Contains(_STRUID) || row["COLUMN_NAME"].ToString().Contains(_STRLASTUID) ||
                    row["COLUMN_NAME"].ToString().Contains(_DTUDT) || row["COLUMN_NAME"].ToString().Contains(_DTLASTUDT))
                    continue;

                string strDataType = Utility.GetDotNetDataType(row["DATA_TYPE"].ToString().ToUpper(), row["PRECISION"].ToString(), row["SCALE"].ToString());

                if (row["IS_IDENTITY"] == "1")
                {
                    primaryKey = row["COLUMN_NAME"].ToString();
                    whereCon += row["COLUMN_NAME"] + " = @" + row["COLUMN_NAME"];
                    continue;
                }

                if (row["COLUMN_NAME"].ToString().ToLower() != "isactive")
                {
                    if (insertCol.ToString() != null && insertCol.ToString() != "")
                    {
                        insertCol.Append(", " + row["COLUMN_NAME"]);
                    }
                    else
                    {
                        insertCol.Append(row["COLUMN_NAME"]);
                    }
                    //if (row["COLUMN_NAME"].ToString().ToLower() == "updated_date" || row["COLUMN_NAME"].ToString().ToLower() == "creation_date")
                    //{
                    //    if (insertValue.ToString() != null && insertValue.ToString() != "")
                    //    {
                    //        insertValue.Append(",");
                    //    }
                    //    insertValue.Append("GETDATE()");
                    //}
                    //else
                    {
                        if (insertValue.ToString() != null && insertValue.ToString() != "")
                        {
                            insertValue.Append(", @" + row["COLUMN_NAME"]);
                        }
                        else
                        {
                            insertValue.Append("@" + row["COLUMN_NAME"]);
                        }
                    }
                    
                }
                //if (row["COLUMN_NAME"].ToString().ToLower() == "updated_date")
                //{
                //    if (UpdateCols.ToString() != null && UpdateCols.ToString() != "")
                //    {
                //        UpdateCols.Append(", " + row["COLUMN_NAME"] + " = GETDATE()");
                //    }
                //    else
                //    {
                //        UpdateCols.Append(row["COLUMN_NAME"] + " = GETDATE()");
                //    }
                //}
                if (row["COLUMN_NAME"].ToString().ToLower() != "creation_date".ToLower() && row["COLUMN_NAME"].ToString().ToLower() != "created_by".ToLower())
                {
                    if (UpdateCols.ToString() != null && UpdateCols.ToString() != "")
                    {
                        UpdateCols.Append(", ");
                    }
                    UpdateCols.Append(row["COLUMN_NAME"] + " = @" + row["COLUMN_NAME"]);
                }
            }

            sb.Append("\n\t\tprivate const string SqlTableName = \"" + strObjectName + "\";");
            sb.Append("\n\t\tprivate const string SqlSelectCommand = \" SELECT * FROM \" + SqlTableName + \" Where isActive = 1 \";");
            sb.Append("\n\t\tprivate const string SqlInsertCommandSQLServer = \" INSERT INTO \" + SqlTableName + \" ( " + insertCol.ToString() + ") OUTPUT Inserted." + primaryKey + " Values(" + insertValue + ")\";");
            sb.Append("\n\t\tprivate const string SqlInsertCommandSQLite = \" INSERT INTO \" + SqlTableName + \" ( " + insertCol.ToString() + ") Values(" + insertValue + ");select last_insert_rowid();\";");
            sb.Append("\n\t\tprivate const string SqlUpdateCommand = \" UPDATE \" + SqlTableName + \" Set " + UpdateCols.ToString() + " where ( " + whereCon + " ) AND  isActive = 1 \";");
            sb.Append("\n\t\tprivate const string SqlDeleteCommand = \" Update \" + SqlTableName + \" Set isActive = 0 where ( " + whereCon + " ) AND isActive = 1 \";");


            sb.Append("\n\n\t\tpublic override int Insert(" + strObjectName + " model)");
            sb.Append("\n\t\t{");
            sb.Append("\n\t\t\tstring SqlInsertCommand = SqlInsertCommandSQLServer;");
            sb.Append("\n\t\t\tif(isSQLite)");
            sb.Append("\n\t\t\t{");
            sb.Append("\n\t\t\t\tSqlInsertCommand=SqlInsertCommandSQLite;");
            sb.Append("\n\t\t\t}");
            sb.Append("\n\t\t\treturn Query<int>(SqlInsertCommand, model).Single();");
            sb.Append("\n\t\t}");

            sb.Append("\n\t\tpublic override int Remove(int id)");
            sb.Append("\n\t\t{");
            sb.Append("\n\t\t\treturn Execute(SqlDeleteCommand, new { " + primaryKey + " = id });");
            sb.Append("\n\t\t}");

            sb.Append("\n\t\tpublic override  " + strObjectName + " FindByID(int id)");
            sb.Append("\n\t\t{");
            sb.Append("\n\t\t\treturn Query<" + strObjectName + ">(SqlSelectCommand + \" AND " + whereCon + " \", new { " + primaryKey + " = id }).FirstOrDefault();");
            sb.Append("\n\t\t}");


            sb.Append("\n\t\tpublic override IEnumerable<" + strObjectName + "> FindByQuery(string query)");
            sb.Append("\n\t\t{");
            sb.Append("\n\t\t\treturn Query<" + strObjectName + ">(SqlSelectCommand + \" AND \" + query);");
            sb.Append("\n\t\t}");


            sb.Append("\n\t\tpublic override IEnumerable<" + strObjectName + "> GetAll()");
            sb.Append("\n\t\t{");
            sb.Append("\n\t\t\treturn Query<" + strObjectName + ">(SqlSelectCommand);");
            sb.Append("\n\t\t}");


            sb.Append("\n\t\tpublic override IEnumerable<" + strObjectName + "> GetTop(int count)");
            sb.Append("\n\t\t{");
            sb.Append("\n\t\t\treturn Query<" + strObjectName + ">(string.Format(\"SELECT TOP {0} * FROM {1}\", count, SqlTableName)).ToList();");
            sb.Append("\n\t\t}");


            sb.Append("\n\t\tpublic override int Update(" + strObjectName + " item)");
            sb.Append("\n\t\t{");
            sb.Append("\n\t\t\treturn Execute(SqlUpdateCommand, item);");
            sb.Append("\n\t\t}");




            sb.Append("\n\t}");
            sb.Append("\n}");

            return sb;
        }
    }
}
