﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Xml;
using BusinessDataLayer;
using System.IO;

/// <summary>
/// Summary description for JaslokMailer
/// </summary>
public class JaslokMailer
{
    public String Body;
    public String FromEmailAddress;
    public String ToEmailAddress;
    public String Subject;
    public String FileName;
    public DataAccessEntities objDAEntities = new DataAccessEntities();

    public JaslokMailer()
    {

    }

    public string SendEmail(string FileName, List<Parameters> lstParameters, string fsEmailAddress, string CcMailId = "")
    {
        try
        {
            EmailBody(FileName, lstParameters);
            MailMessage msg = new MailMessage();
            msg.To.Add(fsEmailAddress);
            if (!string.IsNullOrEmpty(CcMailId))
                msg.CC.Add(CcMailId);

            msg.Subject = this.Subject;
            msg.Body = this.Body;
            msg.IsBodyHtml = true;
            msg.Priority = MailPriority.High;
            msg.From = new MailAddress(this.FromEmailAddress);
            SmtpClient smtp = new SmtpClient("smtp.jaslokhospital.net", 25);
            smtp.Credentials = new System.Net.NetworkCredential("online@jaslokhospital.net", "jIKe%W*cK8");

            smtp.EnableSsl = false;
            //smtp.EnableSsl = true;
            //tls
            smtp.Send(msg);
            return "";
        }
        catch (Exception ex)
        {
            return ex.Message.ToString();
            //HttpContext.Current.Response.Write(ex.StackTrace.ToString());
        }

    }

    public void EmailBody(string FileName, List<Parameters> lstParameters)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemlates/" + FileName + ".xml"));
        XmlNode Subjectnode = doc.DocumentElement.SelectSingleNode("/emailcontent/subject");
        this.Subject = Subjectnode.InnerText.Trim();

        XmlNode BodyNode = doc.DocumentElement.SelectSingleNode("/emailcontent/content");
        this.Body = BodyNode.InnerText.Trim();

        XmlNode FromNode = doc.DocumentElement.SelectSingleNode("/emailcontent/from");
        this.FromEmailAddress = FromNode.InnerText.Trim();

        if (lstParameters.Count > 0)
        {
            foreach (Parameters objParameters in lstParameters)
            {
                this.Body = this.Body.Replace("[" + objParameters.ShortCodeName + "]", objParameters.ShortCodeValue);
            }
        }
    }
    public string SendSms(string FileName, List<Parameters> lstParameters, string contact)
    {
        try
        {
            string strMessege = string.Empty;

            strMessege = SmsBody(FileName, lstParameters);
            WebClient client = new WebClient();
            string baseurl = "http://smsapi.cellapps.com/api/v3/?method=sms&api_key=A316202f45dd36422fdbd773f33a3a44e&to=" + contact + "&sender=Jaslok&message=" + strMessege;
            Stream data = client.OpenRead(baseurl);
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            data.Close();
            reader.Close();
            return s;
        }
        catch (Exception ex)
        {
            return ex.Message.ToString();
        }

    }

    public string SmsBody(string FileName, List<Parameters> lstParameters)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemlates/" + FileName + ".xml"));
        XmlNode BodyNode = doc.DocumentElement.SelectSingleNode("/emailcontent/sms/content");
        this.Body = BodyNode.InnerText.Trim();

        if (lstParameters.Count > 0)
        {
            foreach (Parameters objParameters in lstParameters)
            {
                this.Body = this.Body.Replace("[" + objParameters.ShortCodeName + "]", objParameters.ShortCodeValue);
            }
        }
        return this.Body.ToString();
    }
}

public class Parameters
{
    public string ShortCodeName { get; set; }
    public string ShortCodeValue { get; set; }
}