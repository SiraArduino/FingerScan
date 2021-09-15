using System;
using System.Data;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Threading;
using System.Timers;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;

namespace FingerScan
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

       

        private void Form1_Load(object sender, EventArgs e)
        {

            // Service is started
            WriteToFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Service is started", false);

            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    lblStatus.Text = "Service is running";
                }));
            }
            else
            {
                lblStatus.Text = "Service is running";
            }

            // Setup Timer
            var timer = new System.Timers.Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Enabled = true;

            // Initial Year
            int i = 2019;
            while (i < 2100)
            {
                cbYear.Items.Add(i);
                i += 1;
            }
            cbYear.SelectedIndex = 0;

            // Initial Month
            i = 1;
            while (i < 13)
            {
                cbMonth.Items.Add(i);
                i += 1;
            }
            cbMonth.SelectedIndex = 0;

            // Initial Day
            i = 1;
            while (i < 32)
            {
                cbDay.Items.Add(i);
                i += 1;
            }
            cbDay.SelectedIndex = 0;

            
            //rbtnCustom.Checked = true;
            //rbtnNormal.Checked = true;
            rbtnBackward.Checked = true;
            numericBackward.Value = 2;

            //TimeToWriteFile();
            
        }

        

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            int HH = DateTime.Now.Hour;
            int mm = DateTime.Now.Minute;
            int ss = DateTime.Now.Second;

            // Trigger at 9:30, 13:30, 20:30 and 23:30
            if (((HH == 9 & mm == 30) | (HH == 13 & mm == 30) | (HH == 20 & mm == 30) | (HH == 23 & mm == 30)) & ss == 0)
            {
                TimeToWriteFile();
            }

        }        

        private void TimeToWriteFile()
        {
            // List of IP from all sites
            List<string> ipList = new List<string>();
            ipList.Clear();
            ipList.Add("10.15.17.1"); // AIA-1
            ipList.Add("10.15.17.2"); // AIA-2
            ipList.Add("10.15.17.3"); // AIA-3
            ipList.Add("10.15.17.4"); // AIA-4
            ipList.Add("10.15.17.5"); // AIA-5
            ipList.Add("10.15.17.6"); // AIA-6
            ipList.Add("10.15.17.7"); // AIA-7
            ipList.Add("10.7.13.100"); // Kabin
            ipList.Add("172.19.15.222"); // ESLO
            ipList.Add("172.17.15.221"); // ESL-1
            ipList.Add("172.16.15.222"); // ESN
            ipList.Add("172.21.15.6"); // HKH
            ipList.Add("10.51.8.15"); // EMN-1
            ipList.Add("10.51.8.16"); // EMN-2
            ipList.Add("172.22.15.221"); // HNM
            ipList.Add("10.59.2.221"); // MMR
            ipList.Add("61.7.147.106"); // GTR
            ipList.Add("172.18.15.223"); // ESP
            ipList.Add("182.52.103.197"); // EBI
            
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    lblStatus.Text = "Service is executing...";
                }));
            }
            else
            {
                lblStatus.Text = "Service is executing...";
            }

            // Connection String (ZK)
            string myConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                            @"Data Source=C:\FingerScan\DB\att2000.mdb;" +
                            "Persist Security Info=True;" +
                            "Jet OLEDB:Database Password=;";
            
            try
            {
                File.Copy("C:\\Program Files (x86)\\ZKTime5.0\\att2000.mdb", "C:\\FingerScan\\DB\\att2000.mdb", true);

                // Open OleDb Connection (ZK)
                OleDbConnection myConnection = new OleDbConnection();
                myConnection.ConnectionString = myConnectionString;
                myConnection.Open();               

                // Execute Queries
                OleDbCommand cmd = myConnection.CreateCommand(); // ZK                

                // Check Date Option
                if (rbtnBackward.Checked == true)
                {

                    WriteToFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Query by backward date", false);

                    if (InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(delegate
                        {
                            cmd.CommandText = "SELECT userinfo.badgenumber, checkinout.checktime, machines.ip FROM(CHECKINOUT INNER JOIN userinfo ON checkinout.userid = userinfo.userid) INNER JOIN machines ON checkinout.sensorid = machines.machinenumber WHERE CHECKINOUT.checktime >= Date()-" + numericBackward.Value.ToString() + " and len(userinfo.badgenumber) = 5 ORDER BY checkinout.checktime";
                        }));
                    }
                    else
                    {
                        cmd.CommandText = "SELECT userinfo.badgenumber, checkinout.checktime, machines.ip FROM(CHECKINOUT INNER JOIN userinfo ON checkinout.userid = userinfo.userid) INNER JOIN machines ON checkinout.sensorid = machines.machinenumber WHERE CHECKINOUT.checktime >= Date()-" + numericBackward.Value.ToString() + " and len(userinfo.badgenumber) = 5 ORDER BY checkinout.checktime";
                    }
                }
                else
                {
                    // Set cmd of each time
                    switch (DateTime.Now.Hour)
                    {
                        case 9:
                            WriteToFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Query at 9:30", false);

                            if (rbtnNormal.Checked == true)
                            {
                                cmd.CommandText = "SELECT userinfo.badgenumber, checkinout.checktime, machines.ip FROM(CHECKINOUT INNER JOIN userinfo ON checkinout.userid = userinfo.userid) INNER JOIN machines ON checkinout.sn = machines.sn WHERE((Format(CHECKINOUT.checktime, 'Short Date') = Date() And Format(CHECKINOUT.checktime, 'Short Time') <=#09:35:00#) or (Format(CHECKINOUT.checktime, 'Short Date')= Date()-1 And Format(CHECKINOUT.checktime,'Short Time')>=#23:25:00#)) and len(userinfo.badgenumber) = 5 ORDER BY checkinout.checktime";
                            }
                            else
                            {
                                if (InvokeRequired)
                                {
                                    this.Invoke(new MethodInvoker(delegate
                                    {
                                        cmd.CommandText = "SELECT userinfo.badgenumber, CHECKINOUT.checktime, machines.ip FROM(CHECKINOUT INNER JOIN userinfo ON CHECKINOUT.userid = userinfo.userid) INNER JOIN machines ON CHECKINOUT.sn = machines.sn WHERE(((CHECKINOUT.checktime) >=#" + cbMonth.Text + "/" + cbDay.Text + "/" + cbYear.Text + "# And (CHECKINOUT.checktime)<=#" + DateTime.Now.ToString("MM") + "/" + DateTime.Now.ToString("dd") + "/" + DateTime.Now.ToString("yyyy") + " 09:35:0#)) and len(userinfo.badgenumber) = 5 ORDER BY checkinout.checktime";
                                    }));
                                }
                                else
                                {
                                    cmd.CommandText = "SELECT userinfo.badgenumber, CHECKINOUT.checktime, machines.ip FROM(CHECKINOUT INNER JOIN userinfo ON CHECKINOUT.userid = userinfo.userid) INNER JOIN machines ON CHECKINOUT.sn = machines.sn WHERE(((CHECKINOUT.checktime) >=#" + cbMonth.Text + "/" + cbDay.Text + "/" + cbYear.Text + "# And (CHECKINOUT.checktime)<=#" + DateTime.Now.ToString("MM") + "/" + DateTime.Now.ToString("dd") + "/" + DateTime.Now.ToString("yyyy") + " 09:35:0#)) and len(userinfo.badgenumber) = 5 ORDER BY checkinout.checktime";
                                }
                            }

                            break;
                        case 13:
                            WriteToFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Query at 13:30", false);

                            if (rbtnNormal.Checked == true)
                            {
                                cmd.CommandText = "SELECT userinfo.badgenumber, checkinout.checktime, machines.ip FROM(CHECKINOUT INNER JOIN userinfo ON checkinout.userid = userinfo.userid) INNER JOIN machines ON checkinout.sn = machines.sn WHERE Format(CHECKINOUT.checktime, 'Short Date')= Date() And Format(CHECKINOUT.checktime, 'Short Time')>=#09:25:00# And Format(CHECKINOUT.checktime,'Short Time')<=#13:35:00# and len(userinfo.badgenumber) = 5 ORDER BY checkinout.checktime";
                            }
                            else
                            {
                                if (InvokeRequired)
                                {
                                    this.Invoke(new MethodInvoker(delegate
                                    {
                                        cmd.CommandText = "SELECT userinfo.badgenumber, CHECKINOUT.checktime, machines.ip FROM(CHECKINOUT INNER JOIN userinfo ON CHECKINOUT.userid = userinfo.userid) INNER JOIN machines ON CHECKINOUT.sn = machines.sn WHERE(((CHECKINOUT.checktime) >=#" + cbMonth.Text + "/" + cbDay.Text + "/" + cbYear.Text + "# And (CHECKINOUT.checktime)<=#" + DateTime.Now.ToString("MM") + "/" + DateTime.Now.ToString("dd") + "/" + DateTime.Now.ToString("yyyy") + " 13:35:0#)) and len(userinfo.badgenumber) = 5 ORDER BY checkinout.checktime";
                                    }));
                                }
                                else
                                {
                                    cmd.CommandText = "SELECT userinfo.badgenumber, CHECKINOUT.checktime, machines.ip FROM(CHECKINOUT INNER JOIN userinfo ON CHECKINOUT.userid = userinfo.userid) INNER JOIN machines ON CHECKINOUT.sn = machines.sn WHERE(((CHECKINOUT.checktime) >=#" + cbMonth.Text + "/" + cbDay.Text + "/" + cbYear.Text + "# And (CHECKINOUT.checktime)<=#" + DateTime.Now.ToString("MM") + "/" + DateTime.Now.ToString("dd") + "/" + DateTime.Now.ToString("yyyy") + " 13:35:0#)) and len(userinfo.badgenumber) = 5 ORDER BY checkinout.checktime";
                                }
                            }

                            break;
                        case 20:
                            WriteToFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Query at 20:30", false);

                            if (rbtnNormal.Checked == true)
                            {
                                cmd.CommandText = "SELECT userinfo.badgenumber, checkinout.checktime, machines.ip FROM(CHECKINOUT INNER JOIN userinfo ON checkinout.userid = userinfo.userid) INNER JOIN machines ON checkinout.sn = machines.sn WHERE Format(CHECKINOUT.checktime, 'Short Date')= Date() And Format(CHECKINOUT.checktime, 'Short Time')>=#13:25:00# And Format(CHECKINOUT.checktime,'Short Time')<=#20:35:00# and len(userinfo.badgenumber) = 5 ORDER BY checkinout.checktime";
                            }
                            else
                            {
                                if (InvokeRequired)
                                {
                                    this.Invoke(new MethodInvoker(delegate
                                    {
                                        cmd.CommandText = "SELECT userinfo.badgenumber, CHECKINOUT.checktime, machines.ip FROM(CHECKINOUT INNER JOIN userinfo ON CHECKINOUT.userid = userinfo.userid) INNER JOIN machines ON CHECKINOUT.sn = machines.sn WHERE(((CHECKINOUT.checktime) >=#" + cbMonth.Text + "/" + cbDay.Text + "/" + cbYear.Text + "# And (CHECKINOUT.checktime)<=#" + DateTime.Now.ToString("MM") + "/" + DateTime.Now.ToString("dd") + "/" + DateTime.Now.ToString("yyyy") + " 20:35:0#)) and len(userinfo.badgenumber) = 5 ORDER BY checkinout.checktime";
                                    }));
                                }
                                else
                                {
                                    cmd.CommandText = "SELECT userinfo.badgenumber, CHECKINOUT.checktime, machines.ip FROM(CHECKINOUT INNER JOIN userinfo ON CHECKINOUT.userid = userinfo.userid) INNER JOIN machines ON CHECKINOUT.sn = machines.sn WHERE(((CHECKINOUT.checktime) >=#" + cbMonth.Text + "/" + cbDay.Text + "/" + cbYear.Text + "# And (CHECKINOUT.checktime)<=#" + DateTime.Now.ToString("MM") + "/" + DateTime.Now.ToString("dd") + "/" + DateTime.Now.ToString("yyyy") + " 20:35:0#)) and len(userinfo.badgenumber) = 5 ORDER BY checkinout.checktime";
                                }
                            }

                            break;
                        case 23:
                            WriteToFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Query at 23:30", false);

                            if (rbtnNormal.Checked == true)
                            {
                                cmd.CommandText = "SELECT userinfo.badgenumber, checkinout.checktime, machines.ip FROM(CHECKINOUT INNER JOIN userinfo ON checkinout.userid = userinfo.userid) INNER JOIN machines ON checkinout.sn = machines.sn WHERE Format(CHECKINOUT.checktime, 'Short Date')= Date() And Format(CHECKINOUT.checktime, 'Short Time')>=#20:25:00# And Format(CHECKINOUT.checktime,'Short Time')<=#23:35:00# and len(userinfo.badgenumber) = 5 ORDER BY checkinout.checktime";
                            }
                            else
                            {
                                if (InvokeRequired)
                                {
                                    this.Invoke(new MethodInvoker(delegate
                                    {
                                        cmd.CommandText = "SELECT userinfo.badgenumber, CHECKINOUT.checktime, machines.ip FROM(CHECKINOUT INNER JOIN userinfo ON CHECKINOUT.userid = userinfo.userid) INNER JOIN machines ON CHECKINOUT.sn = machines.sn WHERE(((CHECKINOUT.checktime) >=#" + cbMonth.Text + "/" + cbDay.Text + "/" + cbYear.Text + "# And (CHECKINOUT.checktime)<=#" + DateTime.Now.ToString("MM") + "/" + DateTime.Now.ToString("dd") + "/" + DateTime.Now.ToString("yyyy") + " 23:35:0#)) and len(userinfo.badgenumber) = 5 ORDER BY checkinout.checktime";
                                    }));
                                }
                                else
                                {
                                    cmd.CommandText = "SELECT userinfo.badgenumber, CHECKINOUT.checktime, machines.ip FROM(CHECKINOUT INNER JOIN userinfo ON CHECKINOUT.userid = userinfo.userid) INNER JOIN machines ON CHECKINOUT.sn = machines.sn WHERE(((CHECKINOUT.checktime) >=#" + cbMonth.Text + "/" + cbDay.Text + "/" + cbYear.Text + "# And (CHECKINOUT.checktime)<=#" + DateTime.Now.ToString("MM") + "/" + DateTime.Now.ToString("dd") + "/" + DateTime.Now.ToString("yyyy") + " 23:35:0#)) and len(userinfo.badgenumber) = 5 ORDER BY checkinout.checktime";
                                }
                            }

                            break;
                    }
                }
                OleDbDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection); // close conn after complete (ZK)
                

                // Load the result into a DataTable (ZK)
                DataTable myDataTable = new DataTable();
                myDataTable.Load(reader);               

                // Start to write file
                WriteToFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Start to write file", false);
                
                foreach (DataRow row in myDataTable.Rows)
                {
                    WriteToFile("    " + row["badgenumber"].ToString() + "   " + ((DateTime)row["checktime"]).ToString("yyyy-MM-dd HH:mm:ss") + " " + row["ip"].ToString(), true);
                    if (ipList.Count > 0)
                    {
                        ipList.Remove(row["ip"].ToString());
                    }
                }

                // Check List of IP to send email
                if (ipList.Count > 0)
                {
                    var EA_Mail = new MailMessage();
                    EA_Mail.From = new MailAddress("eafingerscan2020@gmail.com");

                    EA_Mail.Subject = "Finger Scan - Some devices missed";
                    EA_Mail.To.Add(new MailAddress("sira.b@energyabsolute.co.th"));
                    EA_Mail.To.Add(new MailAddress("pornchai.k@energyabsolute.co.th"));
                    EA_Mail.To.Add(new MailAddress("anongrat.p@energyabsolute.co.th"));
                    EA_Mail.IsBodyHtml = true;
                    string body = "";
                    string ipDesc = "";
                    foreach (string ip in ipList)
                    {
                        switch (ip)
                        {
                            case "10.15.17.1":
                                ipDesc = "ประตูประชาสัมพันธ์";
                                break;
                            case "10.15.17.2":
                                ipDesc = "ประตูจัดซื้อ";
                                break;
                            case "10.15.17.3":
                                ipDesc = "ประตูวางแผนลงทุน";
                                break;
                            case "10.15.17.4":
                                ipDesc = "ประตูห้องน้ำชาย";
                                break;
                            case "10.15.17.5":
                                ipDesc = "ประตูห้องน้ำหญิง";
                                break;
                            case "10.15.17.6":
                                ipDesc = "ประตูห้อง Server";
                                break;
                            case "10.15.17.7":
                                ipDesc = "ประตู ESM";
                                break;
                            case "10.7.13.100":
                                ipDesc = "ไซท์ Kabin";
                                break;
                            case "172.19.15.222":
                                ipDesc = "ไซท์ ESLO";
                                break;
                            case "172.17.15.221":
                                ipDesc = "ไซท์ ESL";
                                break;
                            case "172.16.15.222":
                                ipDesc = "ไซท์ ESN";
                                break;
                            case "172.21.15.6":
                                ipDesc = "ไซท์ HKH";
                                break;
                            case "10.51.8.15":
                                ipDesc = "ไซท์ EMN ชั้น 5";
                                break;
                            case "10.51.8.16":
                                ipDesc = "ไซท์ EMN ชั้น 6";
                                break;
                            case "172.22.15.221":
                                ipDesc = "ไซท์ HNM";
                                break;
                            case "10.59.2.221":
                                ipDesc = "ไซท์ MMR";
                                break;
                            case "61.7.147.106":
                                ipDesc = "ไซท์ GTR";
                                break;
                            case "172.18.15.223":
                                ipDesc = "ไซท์ ESP";
                                break;
                            case "182.52.103.197":
                                ipDesc = "ไซท์ EBI";
                                break;
                        }
                        body += ipDesc + "<br />";
                    }
                    EA_Mail.Body = "The following devices were missed.<br />" + body;

                    var EA_Credential = new NetworkCredential("eafingerscan2020@gmail.com", "clear@ea9");
                    var EA_SmtpClient = new SmtpClient();
                    EA_SmtpClient.Port = 587;
                    EA_SmtpClient.UseDefaultCredentials = true;
                    EA_SmtpClient.Credentials = EA_Credential;
                    EA_SmtpClient.Host = "smtp.gmail.com";
                    EA_SmtpClient.EnableSsl = true;

                    EA_SmtpClient.Send(EA_Mail);

                    EA_SmtpClient.Dispose();
                    EA_Mail.Dispose();
                }
                

                // Finished writing file
                WriteToFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Finished writing file", false);

                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        lblStatus.Text = "Service is running";
                    }));
                }
                else
                {
                    lblStatus.Text = "Service is running";
                }

                // Write row count
                //WriteToFile("Total Rows: " + myDataTable.Rows.Count.ToString(), true);

            }
            catch (Exception ex)
            {
                Console.WriteLine("OLEDB Connection FAILED: " + ex.Message);
                WriteToFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Error: " + ex.Message, false); // Write error if it occur
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        lblStatus.Text = ex.Message;
                    }));
                }
                else
                {
                    lblStatus.Text = ex.Message;
                }                
            }
            
        }

        public void WriteToFile(string Message, bool IsFingerFile)
        {
            string path = "";
            string filepath = "";
            //string serverIP = "203.172.50.30"; //10.15.11.177
            //string serverIP = "10.15.11.177";

            try
            {
                // Set path, filepath
                if (IsFingerFile == true)
                {
                    // Finger Scan                
                    path = "C:\\FingerScan";
                    filepath = "C:\\FingerScan\\FingerScan_" + DateTime.Now.ToString("yyyy/MM/dd").Replace("/", "_") + "_" + DateTime.Now.ToString("HH:mm").Replace(":", "_") + ".txt";
                    //path = @"\\" + serverIP + @"\FingerScan";
                    //filepath = @"\\" + serverIP + @"\FingerScan\FingerScan_" + DateTime.Now.ToString("yyyy/MM/dd").Replace("/", "_") + "_" + DateTime.Now.ToString("HH:mm").Replace(":", "_") + ".txt";
                }
                else
                {
                    // Log                
                    path = "C:\\FingerScan\\Log";
                    filepath = "C:\\FingerScan\\Log\\Log_" + DateTime.Now.ToString("yyyy/MM/dd").Replace("/", "_") + ".txt";
                    //path = @"\\" + serverIP + @"\FingerScan\Log";
                    //filepath = @"\\" + serverIP + @"\FingerScan\Log\Log_" + DateTime.Now.ToString("yyyy/MM/dd").Replace("/", "_") + ".txt";                    
                }

                // Check path is exists
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Check filepath is exists
                if (!File.Exists(filepath))
                {
                    
                    using (StreamWriter sw = File.CreateText(filepath)) // Create a file to write to.   
                    {
                        sw.WriteLine(Message);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filepath))
                    {
                        sw.WriteLine(Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("OLEDB Connection FAILED: " + ex.Message);                
            }
            
        }

       
       
    }
}
