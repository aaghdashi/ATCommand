using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Data.Entity;
using System.Threading;

namespace SMS_AT_Command
{
    public partial class SMS : Form
    {

        public SMS()
        {
            InitializeComponent();
        }


        SerialPort myport = new SerialPort();
        string DeviceName = "";


        ContactEntities en = new ContactEntities();
        int _id;
        int _id_message;
        private void SMS_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'contactDataSet.Table' table. You can move, or remove it, as needed.
            this.tableTableAdapter.Fill(this.contactDataSet.Table);
            string[] ports = SerialPort.GetPortNames();

            // Add all port names to the combo box:
            foreach (string port in ports)
            {
                this.comboBox_portname.Items.Add(port);
            }


        }

        private void button_saveContact_Click(object sender, EventArgs e)
        {
            Table tb = new Table();

            tb.Name = textBox_nameContact.Text.Trim();
            tb.Tel = textBox_numberphone.Text.Trim();
            en.Tables.Add(tb);
            en.SaveChanges();
            dataGridView_Contact.DataSource = en.Tables.ToList();

        }

        private void button_edit_Contact_Click(object sender, EventArgs e)
        {
            _id = (int)dataGridView_Contact.CurrentRow.Cells[0].Value;
            Table tb = en.Tables.FirstOrDefault(i => i.ID == _id);
            tb.Name = textBox_nameContact.Text;
            tb.Tel = textBox_numberphone.Text;

            en.SaveChanges();
            dataGridView_Contact.DataSource = en.Tables.ToList();
        }



        private void button_delete_Contact_Click(object sender, EventArgs e)
        {
            _id = (int)dataGridView_Contact.CurrentRow.Cells[0].Value;
            Table tb = en.Tables.FirstOrDefault(i => i.ID == _id);
            en.Tables.Remove(tb);
            en.SaveChanges();

            dataGridView_Contact.DataSource = en.Tables.ToList();

        }

        private void dataGridView_Contact_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            _id = (int)dataGridView_Contact.CurrentRow.Cells[0].Value;
            var sel_con = en.Tables.FirstOrDefault(i => i.ID == _id);

            textBox_nameContact.Text = sel_con.Name;
            textBox_numberphone.Text = sel_con.Tel;
        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            try
            {
                myport.PortName = comboBox_portname.Text;

                myport.BaudRate = int.Parse(comboBox_baudrate.Text);
                switch (comboBox_stopbit.Text)
                {
                    case "1":
                        myport.StopBits = StopBits.One;
                        break;
                    case "1.5":
                        myport.StopBits = StopBits.OnePointFive;
                        break;
                    case "2":
                        myport.StopBits = StopBits.Two;
                        break;
                }
                switch (comboBox_paritybit.Text)
                {
                    case "None":
                        myport.Parity = Parity.None;
                        break;

                    case "Even":
                        myport.Parity = Parity.Even;
                        break;

                    case "Odd":
                        myport.Parity = Parity.Odd;
                        break;
                }
                myport.DataBits = int.Parse(comboBox_databit.Text);
                myport.ReadBufferSize = 10000;
                myport.ReadTimeout = int.Parse(textBox_readtime.Text);
                myport.WriteBufferSize = 10000;
                myport.WriteTimeout = int.Parse(textBox_writetime.Text);
                myport.RtsEnable = true;
                myport.DtrEnable = true;
                myport.Open();
                label_status.Text = "ارتباط بر قرار شد.";
                if (!myport.IsOpen)
                    myport.Open();

                myport.DiscardOutBuffer();//خالی کردن بافر

                myport.WriteLine("AT+cgmm\r");//دستور شناخت مدل دستگاه

                Thread.Sleep(500);

                DeviceName = myport.ReadExisting();
                if (DeviceName.Contains("ERROR"))
                    MessageBox.Show("Device does not support this command or any other problem...");
                else
                {
                    //دستورات زیر برای بیرون کشیدن نام دستگاه از رشته خوانده شده از پورت هست   
                    //(char)13   کاراکتر اینتر!
                    DeviceName = DeviceName.Remove(0, DeviceName.IndexOf((char)13)).Trim();
                    DeviceName = DeviceName.Substring(0, DeviceName.IndexOf((char)13));

                    MessageBox.Show("detected successfully" + Environment.NewLine + "Device Name:" + Environment.NewLine + DeviceName, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    label_name.Text = DeviceName;
                }
                myport.DiscardOutBuffer();

            }
            catch (Exception ex)
            {
                label_status.Text = ex.Message;
            }
        }

        private void button_disconnect_Click(object sender, EventArgs e)
        {
            try
            {
                myport.Close();
                label_status.Text = "ارتباط قطع شد.";
            }
            catch (Exception ex)
            {
                label_status.Text = ex.Message;
            }
        }

        private void comboBox_Contact_SelectedIndexChanged(object sender, EventArgs e)
        {
            label_number.Text = (string)comboBox_Contact.SelectedValue;
        }

        private void button_send1_Click(object sender, EventArgs e)
        {
            if (textBox_telNumber.Text.Trim() != "")
            {
                try
                {
                    if (!myport.IsOpen)
                        myport.Open();

                    //خالی کردن بافر
                    myport.DiscardOutBuffer();
                    myport.DiscardInBuffer();

                    //قراردادن دستگاه در حالت متنی
                    myport.WriteLine("AT+CMGF=1\r");
                    Thread.Sleep(500);

                    //دستور ارسال اس ام اس
                    myport.WriteLine("AT+CMGS=\"" + textBox_telNumber.Text.Trim() + "\"\r");
                    myport.WriteLine(textBox_message1.Text.Trim() + '\x001a');// \x001a  (برای انتهای پیام ctrl+z معادل)                   

                    Thread.Sleep(500);

                    if (myport.ReadExisting().Contains("ERROR"))
                        MessageBox.Show("Device does not support this command or any other problem...");
                    else
                    {
                        MessageBox.Show("Sent successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        SendMessage sm = new SendMessage();
                        sm.Date_Send = DateTime.Now;
                        sm.Message = textBox_message1.Text.Trim();
                        sm.Name_Contact = "ناشناس";
                        sm.Number_Contact = textBox_telNumber.Text.Trim();
                        en.SendMessages.Add(sm);
                        en.SaveChanges();
                    }
                    myport.DiscardOutBuffer();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    myport.Close();
                }
            }
            else
            {
                MessageBox.Show("شماره مورد نظر خود را وارد کنید.");
                textBox_telNumber.Focus();
            }
        }

        private void button_send2_Click(object sender, EventArgs e)
        {
            if (comboBox_Contact.SelectedValue.ToString() != "")
            {
                try
                {
                    if (!myport.IsOpen)
                        myport.Open();

                    //خالی کردن بافر
                    myport.DiscardOutBuffer();
                    myport.DiscardInBuffer();

                    //قراردادن دستگاه در حالت متنی
                    myport.WriteLine("AT+CMGF=1\r");

                    //دستور ارسال اس ام اس
                    myport.WriteLine("AT+CMGS=\"" + label_number.Text.Trim() + "\"\r");
                    myport.WriteLine(textBox_message2.Text.Trim() + '\x001a');// \x001a  (برای انتهای پیام ctrl+z معادل)                   

                    Thread.Sleep(500);

                    if (myport.ReadExisting().Contains("ERROR"))
                        MessageBox.Show("Device does not support this command or any other problem...");
                    else
                    {
                        MessageBox.Show("Sent successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        SendMessage sm = new SendMessage();
                        sm.Date_Send = DateTime.Now;
                        sm.Message = textBox_message2.Text.Trim();
                        sm.Name_Contact = comboBox_Contact.SelectedText.ToString();
                        sm.Number_Contact = comboBox_Contact.SelectedValue.ToString();
                        en.SendMessages.Add(sm);
                        en.SaveChanges();

                    }
                    myport.DiscardOutBuffer();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    myport.Close();
                }
            }
            else
            {
                MessageBox.Show("شماره مورد نظر خود را انتخاب کنید.");
                comboBox_Contact.Focus();
            }

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView_anboh.DataSource = en.Tables.ToList();
            dataGridView_anboh.Columns[0].HeaderText = "کد";
            dataGridView_anboh.Columns[1].HeaderText = "نام مخاطب";
            dataGridView_anboh.Columns[2].HeaderText = "شماره تماس";

            dataGridView_Contact.DataSource = en.Tables.ToList();
            dataGridView_Contact.Columns[0].HeaderText = "کد";
            dataGridView_Contact.Columns[1].HeaderText = "نام مخاطب";
            dataGridView_Contact.Columns[2].HeaderText = "شماره تماس";

            dataGridView_send_message.DataSource = en.SendMessages.ToList();
            dataGridView_send_message.Columns[0].HeaderText = "کد";
            dataGridView_send_message.Columns[1].HeaderText = "تاریخ ارسال";
            dataGridView_send_message.Columns[2].HeaderText = "نام مخاطب";
            dataGridView_send_message.Columns[3].HeaderText = "شماره تماس";
            dataGridView_send_message.Columns[4].HeaderText = "متن پیام";

        }

        private void dataGridView_send_message_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            _id_message = (int)dataGridView_send_message.CurrentRow.Cells[0].Value;
        }

        private void button_Delete_one_Click(object sender, EventArgs e)
        {
            _id_message = (int)dataGridView_send_message.CurrentRow.Cells[0].Value;
            SendMessage sm = en.SendMessages.FirstOrDefault(i => i.Id == _id_message);
            en.SendMessages.Remove(sm);
            en.SaveChanges();

            dataGridView_Contact.DataSource = en.SendMessages.ToList();
        }

        private void button_Delete_all_Click(object sender, EventArgs e)
        {
            var rows = from o in en.SendMessages
                       select o;
            foreach (var row in rows)
            {
                en.SendMessages.Remove(row);
            }
            en.SaveChanges();
            dataGridView_Contact.DataSource = en.SendMessages.ToList();

        }

        private void button_add_all_Click(object sender, EventArgs e)
        {
            listBox_Contact.Items.Clear();
            var rows = from contact in en.Tables select new { contact.Tel };

            foreach (var item in rows)
            {
                listBox_Contact.Items.Add(item.Tel.ToString());
            }
        }

        private void button__remove_all_Click(object sender, EventArgs e)
        {
            listBox_Contact.Items.Clear();
        }

        private void button_remove_selected_Click(object sender, EventArgs e)
        {
            listBox_Contact.Items.RemoveAt(listBox_Contact.SelectedIndex);
        }

        private void button_add_selected_Click(object sender, EventArgs e)
        {
            string _selected_Contact = dataGridView_anboh.CurrentRow.Cells[2].Value.ToString();
            listBox_Contact.Items.Add(_selected_Contact);
        }

        private void dataGridView_anboh_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int _selected_Contact = (int)dataGridView_anboh.CurrentRow.Cells[0].Value;
            var sel_con = en.Tables.FirstOrDefault(i => i.ID == _selected_Contact);

            listBox_Contact.Items.Add(sel_con.Tel);
        }

        private void button_send_messages_Click(object sender, EventArgs e)
        {
            try
            {
                if (!myport.IsOpen)
                    myport.Open();


                foreach (var item in comboBox_Contact.Items)
                //خالی کردن بافر
                {
                    myport.DiscardOutBuffer();
                    myport.DiscardInBuffer();

                    //قراردادن دستگاه در حالت متنی
                    myport.WriteLine("AT+CMGF=1\r");

                    //دستور ارسال اس ام اس
                    myport.WriteLine("AT+CMGS=\"" + item.ToString() + "\"\r");
                    myport.WriteLine(textBox_message.Text.Trim() + '\x001a');// \x001a  (برای انتهای پیام ctrl+z معادل)                   

                    Thread.Sleep(500);

                    if (myport.ReadExisting().Contains("ERROR"))
                        MessageBox.Show("Device does not support this command or any other problem...");
                    else
                    {
                        MessageBox.Show("Sent successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        SendMessage sm = new SendMessage();
                        sm.Date_Send = DateTime.Now;
                        sm.Message = textBox_message.Text.Trim();
                        sm.Name_Contact = en.Tables.FirstOrDefault(i => i.Tel .ToString()== item.ToString()).Name.ToString();
                        sm.Number_Contact = item.ToString();
                        en.SendMessages.Add(sm);
                        en.SaveChanges();

                    }
                    myport.DiscardOutBuffer();
                    Thread.Sleep(3000);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                myport.Close();
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button_read_Click(object sender, EventArgs e)
        {
            try
            {
                if (!myport.IsOpen)
                    myport.Open();

                // خواندن از گوشی SMS
                myport.Write("AT+CMGR=1\n\r");

                System.Threading.Thread.Sleep(1000);

                String sms = myport.ReadExisting();
                textBox_read.Text = sms;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                myport.Close();
            }
        }
    }
}
