using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;

namespace baikiemtra
{
    public partial class Form1 : Form
    {
        Classes.DataProcesser dtBase = new Classes.DataProcesser();
        string maMonDangChon = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnSuaThongTin.Enabled = false;
            try
            {
                HienThiDuLieu();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Không tải được dữ liệu. Hãy chạy file TaoCSDL.sql trước.\n" + ex.Message);
            }
        }

        
        private void HienThiDuLieu()
        {
            dgvDiemThi.DataSource = dtBase.DocBang(
                "SELECT MaMon, TenMon, SoTinChi, DiemThi FROM tblDiemThi ORDER BY MaMon");
            dgvDiemThi.Columns[0].HeaderText = "Mã môn";
            dgvDiemThi.Columns[1].HeaderText = "Tên môn";
            dgvDiemThi.Columns[2].HeaderText = "Số tín chỉ";
            dgvDiemThi.Columns[3].HeaderText = "Điểm thi";
            dgvDiemThi.Columns[1].FillWeight = 180;
            dgvDiemThi.ClearSelection();
        }

        private void XoaTrangForm()
        {
            txtMaMon.Clear();
            txtTenMon.Clear();
            txtSoTin.Clear();
            txtDiemThi.Clear();
            maMonDangChon = "";
            btnSuaThongTin.Enabled = false;
            dgvDiemThi.ClearSelection();
            txtMaMon.Focus();
        }

        
        private bool KiemTraDuLieu(out int soTin, out double diemThi)
        {
            soTin = 0;
            diemThi = 0;
            if (string.IsNullOrWhiteSpace(txtMaMon.Text))
            {
                MessageBox.Show("Vui lòng nhập mã môn.");
                txtMaMon.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtTenMon.Text))
            {
                MessageBox.Show("Vui lòng nhập tên môn.");
                txtTenMon.Focus();
                return false;
            }
            if (!int.TryParse(txtSoTin.Text.Trim(), out soTin) || soTin <= 0)
            {
                MessageBox.Show("Số tín chỉ phải là số nguyên lớn hơn 0.");
                txtSoTin.Focus();
                return false;
            }

            
            string diem = txtDiemThi.Text.Trim().Replace(',', '.');
            if (!double.TryParse(diem, NumberStyles.Float, CultureInfo.InvariantCulture, out diemThi)
                || !(diemThi >= 0 && diemThi <= 10))
            {
                MessageBox.Show("Điểm thi phải là số từ 0 đến 10.");
                txtDiemThi.Focus();
                return false;
            }
            return true;
        }

        private void btnThemVaoDanhSach_Click(object sender, EventArgs e)
        {
            int soTin;
            double diemThi;
            if (!KiemTraDuLieu(out soTin, out diemThi))
                return;

            string maMon = txtMaMon.Text.Trim();
            try
            {
                DataTable dt = dtBase.DocBang("SELECT * FROM tblDiemThi WHERE MaMon = @MaMon",
                    new SqlParameter("@MaMon", maMon));
                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("Mã môn đã tồn tại. Vui lòng nhập mã khác.");
                    txtMaMon.Focus();
                    return;
                }

                string sql = @"INSERT INTO tblDiemThi (MaMon, TenMon, SoTinChi, DiemThi)
                               VALUES (@MaMon, @TenMon, @SoTinChi, @DiemThi)";
                dtBase.CapNhatDuLieu(sql,
                    new SqlParameter("@MaMon", maMon),
                    new SqlParameter("@TenMon", txtTenMon.Text.Trim()),
                    new SqlParameter("@SoTinChi", soTin),
                    new SqlParameter("@DiemThi", diemThi));
                HienThiDuLieu();
                XoaTrangForm();
                MessageBox.Show("Thêm môn học thành công.");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvDiemThi_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvDiemThi.Rows[e.RowIndex].IsNewRow)
                return;

            DataGridViewRow dong = dgvDiemThi.Rows[e.RowIndex];
            txtMaMon.Text = dong.Cells[0].Value.ToString();
            txtTenMon.Text = dong.Cells[1].Value.ToString();
            txtSoTin.Text = dong.Cells[2].Value.ToString();
            txtDiemThi.Text = dong.Cells[3].Value.ToString();
            maMonDangChon = txtMaMon.Text;
            btnSuaThongTin.Enabled = true;
        }

        private void btnSuaThongTin_Click(object sender, EventArgs e)
        {
            if (maMonDangChon == "")
            {
                MessageBox.Show("Vui lòng chọn môn học cần sửa.");
                return;
            }

            int soTin;
            double diemThi;
            if (!KiemTraDuLieu(out soTin, out diemThi))
                return;

            try
            {
                DataTable dt = dtBase.DocBang(
                    "SELECT * FROM tblDiemThi WHERE MaMon = @MaMon AND MaMon <> @MaCu",
                    new SqlParameter("@MaMon", txtMaMon.Text.Trim()),
                    new SqlParameter("@MaCu", maMonDangChon));
                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("Mã môn đã tồn tại. Vui lòng nhập mã khác.");
                    txtMaMon.Focus();
                    return;
                }

                string sql = @"UPDATE tblDiemThi
                               SET MaMon = @MaMon, TenMon = @TenMon,
                                   SoTinChi = @SoTinChi, DiemThi = @DiemThi
                               WHERE MaMon = @MaCu";
                int soDong = dtBase.CapNhatDuLieu(sql,
                    new SqlParameter("@MaMon", txtMaMon.Text.Trim()),
                    new SqlParameter("@TenMon", txtTenMon.Text.Trim()),
                    new SqlParameter("@SoTinChi", soTin),
                    new SqlParameter("@DiemThi", diemThi),
                    new SqlParameter("@MaCu", maMonDangChon));
                if (soDong == 0)
                {
                    MessageBox.Show("Không tìm thấy môn học. Vui lòng chọn lại.");
                    return;
                }

                HienThiDuLieu();
                XoaTrangForm();
                MessageBox.Show("Sửa thông tin thành công.");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult traLoi = MessageBox.Show("Bạn có muốn thoát không?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (traLoi == DialogResult.No)
                e.Cancel = true;
        }
    }
}
