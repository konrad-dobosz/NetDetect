using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetDetect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void SendPingAsync(string address, int timeout, byte[] buffor, PingOptions options)
        {
            Ping ping = new Ping();
            ping.PingCompleted += new PingCompletedEventHandler(EndPing);

            try
            {
                ping.SendAsync(address, timeout, buffor, options, null);
            }
            catch (Exception ex)
            {
                lbx1.Items.Add(ex.Message);
            }
        }

        public void EndPing(object sender, PingCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                lbx1.Items.Add("Błąd: operacja przerwana");
                ((IDisposable)(Ping)sender).Dispose();
                return;
            }
            PingReply reply = e.Reply;
            if (reply.Status == IPStatus.Success)
            {
                lbx1.Items.Add(reply.Address + ": bajtów=" + reply.Buffer.Length + ", czas=" + reply.RoundtripTime + "ms, ttl=" + reply.Options.Ttl);
            }

            ((IDisposable)(Ping)sender).Dispose();

        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            IPAddress startIP = null;
            IPAddress endIP = null;

            lbx1.Items.Clear();

            try
            {
                startIP = IPAddress.Parse(tbxStartIP.Text);
                endIP = IPAddress.Parse (tbxEndIP.Text);
            }
            catch
            {
                MessageBox.Show("Błędnie wprowadzony adres");
            }

            byte[] start = startIP.GetAddressBytes();
            byte[] end = endIP.GetAddressBytes();

            PingOptions options = new PingOptions();
            options.Ttl = 128;
            options.DontFragment = true;
            string data = "aaaaaaaaaaaaaaaaaaaaa";
            byte[] buffor = Encoding.ASCII.GetBytes(data);
            int timeout = 120;

            for (byte octet1 = start[0]; octet1 <= end[0]; octet1++)
            {
                for (byte octet2 = start[1]; octet2 <= end[1]; octet2++)
                {
                    for (byte octet3 = start[2]; octet3 <= end[2]; octet3++)
                    {
                        for (byte octet4 = start[3]; octet4 <= end[3]; octet4++)
                        {
                            IPAddress address = new IPAddress(
                                new byte[] {
                                    octet1, octet2, octet3, octet4
                                });
                            SendPingAsync(address.ToString(), timeout, buffor, options);

                        }
                    }
                }
            }
        }
    }
}
