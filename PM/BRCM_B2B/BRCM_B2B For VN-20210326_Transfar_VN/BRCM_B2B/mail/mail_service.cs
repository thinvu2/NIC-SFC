using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml;

namespace BRCM_B2B.mail
{
    class mail_service
    {

        public string sendMail(string mTo, string mFrom, string mcc, string Title, string Body)
        {

            WebReference.SmtpService client = new WebReference.SmtpService();
            try
            {
                client.WMSendMail(mTo, mFrom, mcc, Title, Body);
                return "Send OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            finally
            {
                client.Dispose();
            }
        }

        public string sendMail(string body_title)
        {

            WebReference.SmtpService client = new WebReference.SmtpService();
            try
            {
                client.WMSendMail("brian.yc.tsai@mail.foxconn.com,linda.hy.wu@mail.foxconn.com,kenneth.yj.cheng@foxconn.com", "", "", body_title, body_title);
                return "Send OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            finally
            {
                client.Dispose();
            }
        }

        public string SReader(string readStr, string Emp)
        {
            string s = getxml(AppDomain.CurrentDomain.BaseDirectory + "Web.config", "SqlConnectionString");
            SqlConnection con = new SqlConnection(s);
            con.Open();
            SqlCommand com = new SqlCommand(readStr, con);
            SqlDataReader dr = com.ExecuteReader();
            bool readOk = dr.Read();
            if (readOk)
            {
                string rt = dr[Emp].ToString();
                con.Close();
                return rt;
            }
            else
            {
                con.Close();
                return "";
            }


        }
        public string getxml(string xmlname, string dbname)
        {
            string constr = "";
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlname);
            foreach (XmlNode xnode in doc.DocumentElement.ChildNodes)  //根節點
            {
                if (xnode.Name == "connectionStrings") //第一子節點head，body
                {
                    foreach (XmlNode xnode1 in xnode.ChildNodes) //第二子節點ResponseItem
                    {
                        string name = xnode1.Attributes["name"].Value;
                        if (name == dbname)
                        {
                            constr = xnode1.Attributes["connectionString"].Value;
                        }
                    }
                }
            }
            return constr;
        }
    }
}
