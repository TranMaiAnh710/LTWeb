/* ==========================================================================
   DataWeb — full setup script (MAIANH / LTWeb HR management)
   Chay lai an toan bat cu luc nao (idempotent voi IF NOT EXISTS).
   ==========================================================================
   1. Tao DB DataWeb
   2. Tao 14 bang (10 EDMX + 4 code-first)
   3. Seed Role, Menu, Role_Menu, PhongBan, ChucVu, NhanVien, Account,
      CaLamViec, HopDong, TinTuc
   ========================================================================== */

IF DB_ID('DataWeb') IS NULL CREATE DATABASE DataWeb;
GO

USE DataWeb;
GO

/* ==========================================================================
   BANG (SCHEMA)
   ========================================================================== */

IF OBJECT_ID('dbo.Role','U') IS NULL
CREATE TABLE dbo.[Role] (
    RoleID   INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(100) NOT NULL
);

IF OBJECT_ID('dbo.Menu','U') IS NULL
CREATE TABLE dbo.[Menu] (
    MenuID    INT IDENTITY(1,1) PRIMARY KEY,
    MenuName  NVARCHAR(150) NOT NULL,
    MenuLink  NVARCHAR(200) NOT NULL,
    MenuOrder INT NOT NULL
);

IF OBJECT_ID('dbo.PhongBan','U') IS NULL
CREATE TABLE dbo.PhongBan (
    MaPB  INT IDENTITY(1,1) PRIMARY KEY,
    TenPB NVARCHAR(150) NOT NULL
);

IF OBJECT_ID('dbo.ChucVu','U') IS NULL
CREATE TABLE dbo.ChucVu (
    MaCV     INT IDENTITY(1,1) PRIMARY KEY,
    TenCV    NVARCHAR(150) NOT NULL,
    PhuCapCV DECIMAL(18,2) NOT NULL
);

IF OBJECT_ID('dbo.NhanVien','U') IS NULL
CREATE TABLE dbo.NhanVien (
    MaNV      INT IDENTITY(1,1) PRIMARY KEY,
    HotenNV   NVARCHAR(150) NOT NULL,
    NgaySinh  DATE NOT NULL,
    GioiTinh  NVARCHAR(10) NOT NULL,
    SDT       INT NOT NULL,          -- se doi ten sang SĐT o cuoi
    LuongCBan DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    TrangThai BIT NULL,
    MaCV      INT NOT NULL,
    MaPB      INT NOT NULL,
    CONSTRAINT FK_NhanVien_ChucVu   FOREIGN KEY (MaCV) REFERENCES dbo.ChucVu(MaCV),
    CONSTRAINT FK_NhanVien_PhongBan FOREIGN KEY (MaPB) REFERENCES dbo.PhongBan(MaPB)
);
GO

/* Doi ten SDT -> SĐT (dung NCHAR de tranh loi codepage khi chay bang sqlcmd) */
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.NhanVien') AND name = 'SDT')
BEGIN
    DECLARE @newName SYSNAME = N'S' + NCHAR(272) + N'T';
    EXEC sp_rename N'dbo.NhanVien.SDT', @newName, N'COLUMN';
END
GO

IF OBJECT_ID('dbo.Account','U') IS NULL
CREATE TABLE dbo.Account (
    UserID       INT IDENTITY(1,1) PRIMARY KEY,
    UserName     NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    IsDeleted    BIT NULL,
    CreatedAt    DATETIME NULL,
    RoleID       INT NOT NULL,
    MaNV         INT NOT NULL,
    CONSTRAINT FK_Account_Role     FOREIGN KEY (RoleID) REFERENCES dbo.[Role](RoleID),
    CONSTRAINT FK_Account_NhanVien FOREIGN KEY (MaNV)   REFERENCES dbo.NhanVien(MaNV)
);

IF OBJECT_ID('dbo.Role_Menu','U') IS NULL
CREATE TABLE dbo.Role_Menu (
    RMID      INT IDENTITY(1,1) PRIMARY KEY,
    RoleID    INT NOT NULL,
    MenuID    INT NOT NULL,
    IsVisible BIT NOT NULL,
    IsDeleted BIT NULL,
    CONSTRAINT FK_RoleMenu_Role FOREIGN KEY (RoleID) REFERENCES dbo.[Role](RoleID) ON DELETE CASCADE,
    CONSTRAINT FK_RoleMenu_Menu FOREIGN KEY (MenuID) REFERENCES dbo.[Menu](MenuID) ON DELETE CASCADE
);

/* ---------- CaLamViec (phai co truoc ChamCong vi ChamCong.MaCa FK sang no) ---------- */
IF OBJECT_ID('dbo.CaLamViec','U') IS NULL
CREATE TABLE dbo.CaLamViec (
    MaCa        INT IDENTITY(1,1) PRIMARY KEY,
    TenCa       NVARCHAR(100) NOT NULL,
    GioBatDau   TIME(0) NOT NULL,
    GioKetThuc  TIME(0) NOT NULL,
    GhiChu      NVARCHAR(255) NULL,
    TrangThai   BIT NOT NULL DEFAULT 1
);

IF OBJECT_ID('dbo.ChamCong','U') IS NULL
CREATE TABLE dbo.ChamCong (
    MaChamCong  INT IDENTITY(1,1) PRIMARY KEY,
    Ngay        DATE NOT NULL,
    ThoiGianVao DATETIME NOT NULL,
    ThoiGianRa  DATETIME NOT NULL,
    SoGioLam    TIME(7) NULL,
    GioDiMuon   TIME(7) NULL,
    GioVeSom    TIME(7) NULL,
    GioTangCa   TIME(7) NULL,
    TrangThai   NVARCHAR(MAX) NOT NULL,
    MaNV        INT NOT NULL,
    MaCa        INT NULL,
    CONSTRAINT FK_ChamCong_NhanVien  FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien(MaNV) ON DELETE CASCADE,
    CONSTRAINT FK_ChamCong_CaLamViec FOREIGN KEY (MaCa) REFERENCES dbo.CaLamViec(MaCa)
);

IF OBJECT_ID('dbo.NhatKyLuong','U') IS NULL
CREATE TABLE dbo.NhatKyLuong (
    IDNhatKy   INT IDENTITY(1,1) PRIMARY KEY,
    MaNV       INT NOT NULL,
    LuongCBan  DECIMAL(18,2) NOT NULL,
    NgayApDung DATE NOT NULL,
    GhiChu     VARCHAR(MAX) NULL,
    CONSTRAINT FK_NhatKyLuong_NhanVien FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien(MaNV) ON DELETE CASCADE
);

IF OBJECT_ID('dbo.PhieuLuong','U') IS NULL
CREATE TABLE dbo.PhieuLuong (
    MaBL           INT IDENTITY(1,1) PRIMARY KEY,
    MaNV           INT NOT NULL,
    IDNhatKy       INT NOT NULL,
    Thang          INT NOT NULL,
    Nam            INT NOT NULL,
    LuongCBan      DECIMAL(18,2) NOT NULL,
    Luong1NgayCong DECIMAL(18,2) NOT NULL,
    HeSoTangCa     DECIMAL(5,2)  NOT NULL,
    Luong1hTangCa  DECIMAL(18,2) NOT NULL,
    Thuong         DECIMAL(18,2) NULL,
    Phat           DECIMAL(18,2) NULL,
    SoNgayCong     INT NOT NULL,
    SoGioTangCa    FLOAT NULL,
    BHYT           DECIMAL(18,2) NOT NULL,
    BHXH           DECIMAL(18,2) NOT NULL,
    Thue           DECIMAL(18,2) NOT NULL,
    TongLuong      DECIMAL(18,2) NOT NULL,
    CreatedAt      DATETIME NULL,
    TrangThai      BIT NULL,
    MaChamCong     INT NOT NULL,
    CONSTRAINT FK_PhieuLuong_NhanVien    FOREIGN KEY (MaNV)       REFERENCES dbo.NhanVien(MaNV),
    CONSTRAINT FK_PhieuLuong_NhatKyLuong FOREIGN KEY (IDNhatKy)   REFERENCES dbo.NhatKyLuong(IDNhatKy),
    CONSTRAINT FK_PhieuLuong_ChamCong    FOREIGN KEY (MaChamCong) REFERENCES dbo.ChamCong(MaChamCong) ON DELETE CASCADE
);

IF OBJECT_ID('dbo.HopDong','U') IS NULL
CREATE TABLE dbo.HopDong (
    MaHD         INT IDENTITY(1,1) PRIMARY KEY,
    SoHopDong    NVARCHAR(50) NOT NULL,
    MaNV         INT NOT NULL,
    LoaiHopDong  NVARCHAR(100) NOT NULL,
    NgayBatDau   DATE NOT NULL,
    NgayKetThuc  DATE NULL,
    LuongHopDong DECIMAL(18,2) NOT NULL,
    NgayKy       DATE NOT NULL,
    NguoiKy      NVARCHAR(150) NULL,
    NoiDung      NVARCHAR(MAX) NULL,
    TrangThai    NVARCHAR(50) NOT NULL DEFAULT N'HieuLuc',
    CreatedAt    DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_HopDong_NhanVien FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien(MaNV)
);

IF OBJECT_ID('dbo.TinTuc','U') IS NULL
CREATE TABLE dbo.TinTuc (
    MaTin      INT IDENTITY(1,1) PRIMARY KEY,
    TieuDe     NVARCHAR(255) NOT NULL,
    TomTat     NVARCHAR(500) NULL,
    NoiDung    NVARCHAR(MAX) NULL,
    HinhAnh    NVARCHAR(300) NULL,
    LoaiTin    NVARCHAR(50) NOT NULL DEFAULT N'TinTuc',
    NgayDang   DATETIME NOT NULL DEFAULT GETDATE(),
    TacGia     NVARCHAR(100) NULL,
    NoiBat     BIT NOT NULL DEFAULT 0,
    IsDeleted  BIT NOT NULL DEFAULT 0
);

IF OBJECT_ID('dbo.LienHe','U') IS NULL
CREATE TABLE dbo.LienHe (
    MaLH        INT IDENTITY(1,1) PRIMARY KEY,
    HoTen       NVARCHAR(150) NOT NULL,
    Email       NVARCHAR(150) NOT NULL,
    SoDienThoai NVARCHAR(20) NULL,
    TieuDe      NVARCHAR(200) NOT NULL,
    NoiDung     NVARCHAR(MAX) NOT NULL,
    NgayGui     DATETIME NOT NULL DEFAULT GETDATE(),
    DaXuLy      BIT NOT NULL DEFAULT 0
);
GO

/* ==========================================================================
   SEED DATA
   ========================================================================== */

/* ---------- Role: 2/4/5/6/7 khop voi switch case cua LoginController ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.[Role])
BEGIN
    SET IDENTITY_INSERT dbo.[Role] ON;
    INSERT INTO dbo.[Role](RoleID, RoleName) VALUES
        (2, N'TruongPhong'),
        (4, N'KeToan'),
        (5, N'NhanSu'),
        (6, N'NhanVien'),
        (7, N'GiamDoc');
    SET IDENTITY_INSERT dbo.[Role] OFF;
END

/* ---------- PhongBan ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.PhongBan)
    INSERT INTO dbo.PhongBan(TenPB) VALUES
        (N'Phong Hanh chinh'),
        (N'Phong Ky thuat'),
        (N'Phong Kinh doanh'),
        (N'Phong Ke toan');

/* ---------- ChucVu ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.ChucVu)
    INSERT INTO dbo.ChucVu(TenCV, PhuCapCV) VALUES
        (N'Giam doc',      5000000),
        (N'Truong phong',  3000000),
        (N'Nhan vien',     1000000),
        (N'Thuc tap sinh',  500000);

/* ---------- NhanVien (dung dynamic SQL vi co ky tu Đ trong ten cot) ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.NhanVien)
BEGIN
    DECLARE @sdt SYSNAME = N'S' + NCHAR(272) + N'T';
    DECLARE @sql NVARCHAR(MAX) = N'
        INSERT INTO dbo.NhanVien(HotenNV, NgaySinh, GioiTinh, ' + QUOTENAME(@sdt) + N', LuongCBan, CreatedAt, TrangThai, MaCV, MaPB) VALUES
            (N''Nguyen Van A'', ''1990-05-15'', N''Nam'', 912345678, 15000000, GETDATE(), 1, 1, 1),
            (N''Tran Thi B'',   ''1992-08-20'', N''Nu'',  913456789, 10000000, GETDATE(), 1, 2, 2),
            (N''Le Van C'',     ''1995-11-10'', N''Nam'', 914567890,  8000000, GETDATE(), 1, 3, 3),
            (N''Pham Thi D'',   ''1998-03-25'', N''Nu'',  915678901,  6000000, GETDATE(), 1, 3, 4);
    ';
    EXEC sp_executesql @sql;
END

/* ---------- Account (mat khau = 123456, plain text vi LoginController so sanh truc tiep) ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Account)
    INSERT INTO dbo.Account(UserName, PasswordHash, IsDeleted, CreatedAt, RoleID, MaNV) VALUES
        (N'truongphong', N'123456', 0, GETDATE(), 2, 1),
        (N'ketoan',      N'123456', 0, GETDATE(), 4, 2),
        (N'nhansu',      N'123456', 0, GETDATE(), 5, 3),
        (N'nhanvien',    N'123456', 0, GETDATE(), 6, 4),
        (N'giamdoc',     N'123456', 0, GETDATE(), 7, 1);

/* ---------- CaLamViec ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.CaLamViec)
    INSERT INTO dbo.CaLamViec(TenCa, GioBatDau, GioKetThuc, GhiChu) VALUES
        (N'Ca sang',       '08:00', '12:00', N'Ca lam viec buoi sang'),
        (N'Ca chieu',      '13:00', '17:00', N'Ca lam viec buoi chieu'),
        (N'Ca toi',        '18:00', '22:00', N'Ca lam viec buoi toi'),
        (N'Ca hanh chinh', '08:00', '17:00', N'Ca hanh chinh 8h - 17h');

/* ---------- HopDong ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.HopDong)
    INSERT INTO dbo.HopDong(SoHopDong, MaNV, LoaiHopDong, NgayBatDau, NgayKetThuc, LuongHopDong, NgayKy, NguoiKy, TrangThai) VALUES
        (N'HD-2025-001', 1, N'Khong xac dinh thoi han', '2024-01-01', NULL,         15000000, '2023-12-20', N'Giam doc',              N'HieuLuc'),
        (N'HD-2025-002', 2, N'Xac dinh thoi han',       '2024-06-01', '2027-06-01', 10000000, '2024-05-15', N'Giam doc',              N'HieuLuc'),
        (N'HD-2025-003', 3, N'Xac dinh thoi han',       '2025-01-01', '2026-12-31',  8000000, '2024-12-20', N'Giam doc',              N'HieuLuc'),
        (N'HD-2025-004', 4, N'Thu viec',                '2025-06-01', '2025-08-31',  6000000, '2025-05-25', N'Truong phong nhan su',  N'HieuLuc');

/* ---------- TinTuc (trang chu public) ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.TinTuc)
    INSERT INTO dbo.TinTuc(TieuDe, TomTat, NoiDung, LoaiTin, TacGia, NoiBat) VALUES
        (N'CompanyOS ra mat he thong quan ly nhan su moi',
         N'He thong quan ly nhan su hoan toan moi giup toi uu quy trinh HR va nang cao trai nghiem nhan vien.',
         N'Trong nam 2026, CompanyOS chinh thuc trien khai he thong quan ly nhan su noi bo the he moi. He thong bao gom cac phan he: quan ly ho so nhan vien, hop dong lao dong, cham cong theo ca, va bao cao thong ke thoi gian thuc.',
         N'TinTuc', N'Ban Truyen Thong', 1),

        (N'Tuyen dung 15 vi tri lap trinh vien Q2/2026',
         N'CompanyOS mo dot tuyen dung 15 vi tri lap trinh vien cac cong nghe .NET, Java, ReactJS trong quy 2 nam 2026.',
         N'Chung toi dang tim kiem cac ung vien nhiet huyet cho cac vi tri: Senior .NET Developer (5 vi tri), Junior Java Developer (5 vi tri), Frontend ReactJS Developer (5 vi tri). Uu tien ung vien co kinh nghiem lam viec voi kien truc microservices.',
         N'TinTuc', N'Phong Nhan Su', 1),

        (N'Su kien Team Building he 2026',
         N'Toan cong ty se to chuc chuyen team building tai Da Nang tu ngay 20/07/2026 den 22/07/2026.',
         N'Ban Truyen Thong tran trong thong bao lich chuyen di team building he 2026 duoc to chuc tai Da Nang. Xe khoi hanh luc 5h sang ngay 20/07/2026 tai san cong ty. De nghi CBNV chuan bi day du hanh ly va tuan thu lich trinh chi tiet gui qua email.',
         N'SuKien', N'Ban Truyen Thong', 1),

        (N'Chinh sach nghi phep moi ap dung tu 01/08/2026',
         N'Cong ty dieu chinh chinh sach nghi phep, tang so ngay phep nam len 15 ngay cho nhan vien co tham nien tren 2 nam.',
         N'Ke tu ngay 01/08/2026, CompanyOS ap dung chinh sach nghi phep moi. Nhan vien co tham nien tren 2 nam se duoc huong 15 ngay phep/nam thay vi 12 ngay nhu truoc day. Nhan vien tham nien tren 5 nam duoc huong 18 ngay phep/nam.',
         N'ThongBao', N'Phong Nhan Su', 0),

        (N'Le trao thuong Nhan vien xuat sac Quy 1/2026',
         N'Cong ty trang trong to chuc le trao thuong nhan vien xuat sac quy 1 nam 2026 voi 12 ca nhan duoc vinh danh.',
         N'Sang ngay 15/04/2026, tai hoi truong tang 5, Ban Giam doc CompanyOS da trao thuong cho 12 nhan vien xuat sac trong quy 1/2026. Moi ca nhan nhan giay khen kem phan thuong tri gia 5.000.000 VND.',
         N'SuKien', N'Ban Truyen Thong', 0),

        (N'Chuong trinh dao tao ky nang lanh dao 2026',
         N'CompanyOS phoi hop voi doi tac dao tao chuyen nghiep to chuc chuong trinh Leadership 2026 cho 30 truong nhom.',
         N'Chuong trinh keo dai 3 thang tu 01/09/2026, bao gom 12 buoi hoc mixed online va offline. Doi tuong: truong nhom, truong phong tiem nang. Dang ky truoc 20/08/2026 qua Phong Nhan Su.',
         N'ThongBao', N'Phong Dao Tao', 0);

/* ---------- Menu ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.[Menu])
BEGIN
    SET IDENTITY_INSERT dbo.[Menu] ON;
    INSERT INTO dbo.[Menu](MenuID, MenuName, MenuLink, MenuOrder) VALUES
        /* --- TruongPhong (RoleID 2) --- */
        (10, N'Dashboard',          N'TruongPhong/Dashboard/Index',    1),
        (11, N'Quan ly tai khoan',  N'TruongPhong/Account/Index',      2),
        (12, N'Quan ly nhan vien',  N'NhanSu/Employee/Index',          3),
        (13, N'Quan ly hop dong',   N'TruongPhong/HopDong/Index',      4),
        (14, N'Quan ly cham cong',  N'NhanSu/ChamCong/Index',          5),
        (15, N'Quan ly ca lam viec',N'TruongPhong/CaLamViec/Index',    6),
        (16, N'Quan ly phong ban',  N'TruongPhong/PhongBan/Index',     7),
        (17, N'Quan ly chuc vu',    N'TruongPhong/ChucVu/Index',       8),
        (18, N'Phan quyen',         N'TruongPhong/Menu/Index',         9),
        (19, N'Bao cao thong ke',   N'TruongPhong/BaoCao/Index',      10),
        (20, N'Quan ly tin tuc',    N'TruongPhong/TinTuc/Index',      11),
        (21, N'Lien he',            N'TruongPhong/LienHe/Index',      12),
        /* --- Nhan su (RoleID 5) --- */
        (30, N'Dashboard',           N'NhanSu/Dashboard/Index',        1),
        (31, N'Quan ly nhan vien',   N'NhanSu/Employee/Index',         2),
        (32, N'Quan ly hop dong',    N'NhanSu/HopDong/Index',          3),
        (33, N'Quan ly cham cong',   N'NhanSu/ChamCong/Index',         4),
        (34, N'Quan ly ca lam viec', N'NhanSu/CaLamViec/Index',        5),
        (35, N'Tra cuu nhan su',     N'GiamDoc/TraCuu/NhanVien',       6),
        /* --- Giam doc (RoleID 7) --- */
        (40, N'Dashboard',        N'GiamDoc/Dashboard/Index',       1),
        (41, N'Tra cuu nhan su',  N'GiamDoc/TraCuu/NhanVien',       2),
        (42, N'Tra cuu phong ban',N'GiamDoc/TraCuu/PhongBan',       3),
        (43, N'Tra cuu chuc vu',  N'GiamDoc/TraCuu/ChucVu',         4),
        (44, N'Bao cao thong ke', N'GiamDoc/BaoCao/Index',          5),
        (45, N'Dang bai tin tuc', N'GiamDoc/TinTuc/Index',          6),
        (46, N'Quan ly menu',     N'GiamDoc/Menu/Index',            7),
        /* --- Nhan vien (RoleID 6) --- */
        (50, N'Dashboard',         N'NhanVien/Dashboard/Index',     1),
        (51, N'Ho so ca nhan',     N'NhanVien/HoSo/Index',          2),
        (52, N'Hop dong cua toi',  N'NhanVien/HopDong/Index',       3),
        (53, N'Cham cong cua toi', N'NhanVien/ChamCong/Index',      4),
        (54, N'Doi mat khau',      N'NhanVien/DoiMatKhau/Index',    5);
    SET IDENTITY_INSERT dbo.[Menu] OFF;
END

/* ---------- Role_Menu ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Role_Menu)
BEGIN
    INSERT INTO dbo.Role_Menu(RoleID, MenuID, IsVisible, IsDeleted)
    SELECT 2, MenuID, 1, 0 FROM dbo.[Menu] WHERE MenuID BETWEEN 10 AND 21;

    INSERT INTO dbo.Role_Menu(RoleID, MenuID, IsVisible, IsDeleted)
    SELECT 5, MenuID, 1, 0 FROM dbo.[Menu] WHERE MenuID BETWEEN 30 AND 35;

    INSERT INTO dbo.Role_Menu(RoleID, MenuID, IsVisible, IsDeleted)
    SELECT 7, MenuID, 1, 0 FROM dbo.[Menu] WHERE MenuID BETWEEN 40 AND 46;

    INSERT INTO dbo.Role_Menu(RoleID, MenuID, IsVisible, IsDeleted)
    SELECT 6, MenuID, 1, 0 FROM dbo.[Menu] WHERE MenuID BETWEEN 50 AND 54;
END
GO

/* ==========================================================================
   VERIFY
   ========================================================================== */
PRINT '=== DataWeb ready ===';
SELECT 'Tables' AS Info, COUNT(*) AS Cnt FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'
UNION ALL SELECT 'Roles',      COUNT(*) FROM dbo.[Role]
UNION ALL SELECT 'Accounts',   COUNT(*) FROM dbo.Account
UNION ALL SELECT 'Menus',      COUNT(*) FROM dbo.[Menu]
UNION ALL SELECT 'Role_Menu',  COUNT(*) FROM dbo.Role_Menu
UNION ALL SELECT 'PhongBan',   COUNT(*) FROM dbo.PhongBan
UNION ALL SELECT 'ChucVu',     COUNT(*) FROM dbo.ChucVu
UNION ALL SELECT 'NhanVien',   COUNT(*) FROM dbo.NhanVien
UNION ALL SELECT 'CaLamViec',  COUNT(*) FROM dbo.CaLamViec
UNION ALL SELECT 'HopDong',    COUNT(*) FROM dbo.HopDong
UNION ALL SELECT 'TinTuc',     COUNT(*) FROM dbo.TinTuc
UNION ALL SELECT 'LienHe',     COUNT(*) FROM dbo.LienHe;
