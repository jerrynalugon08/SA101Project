using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SA101Project
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            const string correctUsername = "admin";
            const string correctPassword = "admin";

            string username = txtUser.Text;
            string password = txtPass.Text;

            if (username == correctUsername && password == correctPassword)
            {
                MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Form3 f3 = new Form3();
                f3.Show();

                this.Close();
            }
            else
            {
                txtPass.Clear();

                MessageBox.Show("Invalid username or password. Please try again.",
                                "Login Failed",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtUser.Focus();
            }
        }
    }

    }

