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


namespace Ktra
{
    public partial class Form1 : Form
    {
        Classes.DataProcessor dt = new Classes.DataProcessor();
        Classes.Function ft = new Classes.Function();
        public Form1()
        {
            InitializeComponent();
            txtLuongThang.ShortcutsEnabled = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(DialogResult.Yes == MessageBox.Show("Bạn có muốn xóa công việc này không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                txtTenCongViec.Clear();
                txtLuongThang.Clear();
                txtMaCongViec.Focus();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnSua.Enabled = false;
            btnXoa.Enabled = false;

            try
            {
                TaiDuLieu();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải danh sách công việc: " + ex.Message);
            }
        }

        private void TaiDuLieu()
        {
            DataTable dtBang = dt.DocBang("SELECT MaCongViec, TenCongViec, LuongThang FROM CongViec");
            dgvCongViec.DataSource = dtBang;

            dgvCongViec.Columns[0].HeaderText = "Ma cong viec";
            dgvCongViec.Columns[1].HeaderText = "Ten cong viec";
            dgvCongViec.Columns[2].HeaderText = "Luong Thang";

            dgvCongViec.Columns[0].Width = 100;
            dgvCongViec.Columns[1].Width = 100;
            dgvCongViec.Columns[2].Width = 100;

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Ban co muon thoat khong?", "Thong bao", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void txtLuongThang_MouseLeave(object sender, EventArgs e)
        {
 
        }

        private void txtLuongThang_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && (e.KeyChar< '0' || e.KeyChar > '9'))
            {
                e.Handled = true;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaCongViec.Text)){
                MessageBox.Show("Vui long nhap ma cong viec");
                txtMaCongViec.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtTenCongViec.Text))
            {
                MessageBox.Show("Vui long nhap ten cong viec");
                txtTenCongViec.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtLuongThang.Text))
            {
                MessageBox.Show("Vui long nhap luong thang:");
                txtLuongThang.Focus();
                return;
            }

            int luongThang;
            if (!int.TryParse(txtLuongThang.Text.Trim(), out luongThang) || luongThang < 0)
            {
                MessageBox.Show("Lương tháng phải là số nguyên không âm hợp lệ.");
                txtLuongThang.Focus();
                return;
            }

            string maCV = txtMaCongViec.Text.Trim();
            try
            {
                DataTable bang = dt.DocBang(
                    "SELECT MaCongViec FROM CongViec WHERE MaCongViec = @MaCongViec",
                    new SqlParameter("@MaCongViec", maCV));
                if (bang.Rows.Count > 0)
                {
                    MessageBox.Show("Ma cong viec da ton tai");
                    txtMaCongViec.Focus();
                    return;
                }

                string sql = "Insert into CongViec(MaCongViec,TenCongViec,LuongThang) Values (@MaCongViec,@TenCongViec,@LuongThang)";
                dt.CapNhatDuLieu(sql, new SqlParameter("@MaCongViec", maCV),
                    new SqlParameter("@TenCongViec", txtTenCongViec.Text.Trim()),
                    new SqlParameter("@LuongThang", SqlDbType.Int) { Value = luongThang });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể thêm công việc: " + ex.Message);
                return;
            }

            try
            {
                TaiDuLieu();
                MessageBox.Show("Thêm công việc thành công.");
                txtMaCongViec.Clear();
                txtTenCongViec.Clear();
                txtLuongThang.Clear();
                txtMaCongViec.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi!!");
            }

        }

        private void dgvCongViec_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvCongViec.Rows.Count)
            {
                return;
            }

            DataGridViewRow row = dgvCongViec.Rows[e.RowIndex];
            if (row.IsNewRow)
            {
                return;
            }

            txtMaCongViec.Text = Convert.ToString(row.Cells[0].Value);
            txtTenCongViec.Text = Convert.ToString(row.Cells[1].Value);
            txtLuongThang.Text = Convert.ToString(row.Cells[2].Value);

            btnSua.Enabled = true;
            btnXoa.Enabled = true;


        }
    }
}
