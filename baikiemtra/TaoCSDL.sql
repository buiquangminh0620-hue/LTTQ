-- Câu 1: tạo CSDL, bảng và 3 dòng dữ liệu mẫu.
-- Chạy bằng SQL Server Management Studio, chọn Windows Authentication.
USE master;
GO
IF DB_ID(N'DuLieu') IS NULL
    CREATE DATABASE DuLieu;
GO
USE DuLieu;
GO
IF OBJECT_ID(N'dbo.tblDiemThi', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblDiemThi
    (
        MaMon nvarchar(20) NOT NULL PRIMARY KEY,
        TenMon nvarchar(100) NOT NULL,
        SoTinChi int NOT NULL CHECK (SoTinChi > 0),
        DiemThi float NOT NULL CHECK (DiemThi >= 0 AND DiemThi <= 10)
    );
END;
GO
-- Chỉ thêm mẫu khi bảng rỗng, không ghi đè dữ liệu đã có.
IF NOT EXISTS (SELECT 1 FROM dbo.tblDiemThi)
BEGIN
    INSERT INTO dbo.tblDiemThi (MaMon, TenMon, SoTinChi, DiemThi)
    VALUES (N'M01', N'Lập trình trực quan', 3, 8.5),
           (N'M02', N'Cơ sở dữ liệu', 3, 7.0),
           (N'M03', N'Toán cao cấp', 2, 6.5);
END;
GO
SELECT MaMon, TenMon, SoTinChi, DiemThi FROM dbo.tblDiemThi;
