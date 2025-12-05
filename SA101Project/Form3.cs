using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;



namespace SA101Project
{
    public partial class Form3 : Form

    {
        SqlConnection con = new SqlConnection(
        @"Data Source=DESKTOP-MYPC;Initial Catalog=SA101Project;Integrated Security=True"
    );


        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void barcode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM drugs WHERE barcode=@barcode", con);
                cmd.Parameters.AddWithValue("@barcode", txtBarcode.Text);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    txtDrugName.Text = dr["drug_name"].ToString();
                    txtPrice.Text = dr["price"].ToString();
                }
                else
                {
                    txtDrugName.Text = "";
                    txtPrice.Text = "";
                }

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {

            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                MessageBox.Show("Invalid price. Enter a numeric value.");
                return;
            }
            if (!int.TryParse(txtQuantity.Text, out int qty))
            {
                MessageBox.Show("Invalid quantity. Enter a numeric value.");
                return;
            }

            decimal total = price * qty;

            dataGridView1.Rows.Add(
                txtBarcode.Text,
                txtDrugName.Text,
                price,
                qty,
                total,
                "Delete"
            );

            ComputeTotal();

            txtBarcode.Clear();
            txtDrugName.Clear();
            txtPrice.Clear();
            txtQuantity.Clear();

        }

        private void txtPayment_TextChanged(object sender, EventArgs e)
        {
            if (txtPayment.Text == "") return;

            decimal total = Convert.ToDecimal(txtTotal.Text);
            decimal payment = Convert.ToDecimal(txtPayment.Text);

            txtBalance.Text = (payment - total).ToString("0.00");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                SqlCommand saleCmd = new SqlCommand(
                    "INSERT INTO sales(total, payment, balance) OUTPUT INSERTED.sale_id " +
                    "VALUES(@total, @payment, @balance)",
                    con
                );

                saleCmd.Parameters.AddWithValue("@total", txtTotal.Text);
                saleCmd.Parameters.AddWithValue("@payment", txtPayment.Text);
                saleCmd.Parameters.AddWithValue("@balance", txtBalance.Text);

                int saleID = (int)saleCmd.ExecuteScalar();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;

                    SqlCommand itemCmd = new SqlCommand(
                        "INSERT INTO sales_items(sale_id, barcode, drug_name, price, qty, total) " +
                        "VALUES(@sid, @barcode, @name, @price, @qty, @total)",
                        con
                    );

                    itemCmd.Parameters.AddWithValue("@sid", saleID);
                    itemCmd.Parameters.AddWithValue("@barcode", row.Cells[0].Value);
                    itemCmd.Parameters.AddWithValue("@name", row.Cells[1].Value);
                    itemCmd.Parameters.AddWithValue("@price", row.Cells[2].Value);
                    itemCmd.Parameters.AddWithValue("@qty", row.Cells[3].Value);
                    itemCmd.Parameters.AddWithValue("@total", row.Cells[4].Value);

                    itemCmd.ExecuteNonQuery();
                }

                MessageBox.Show("Invoice Saved!");
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            txtTotal.Clear();
            txtPayment.Clear();
            txtBalance.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {
                dataGridView1.Rows.RemoveAt(e.RowIndex);
                ComputeTotal();
            }
        }

        private void ComputeTotal()
        {
            decimal sum = 0;


            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                sum += Convert.ToDecimal(row.Cells[4].Value);
            }

            txtTotal.Text = sum.ToString("0.00");
        }

    }
}
