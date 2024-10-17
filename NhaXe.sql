﻿DROP DATABASE IF EXISTS NHAXE_QUOCVUONG
go


create database NHAXE_QUOCVUONG
go 
use NHAXE_QUOCVUONG
go
CREATE TABLE [DiaDiem] (
  [ID_DIADIEM] int IDENTITY(1,1) PRIMARY KEY ,
  [TEN_TINH_THANH] nvarchar(255) NOT NULL
  )
GO

CREATE TABLE [TuyenDuong] (
  [ID_TUYEN] int IDENTITY(1,1)  PRIMARY KEY,
  [TEN_TUYEN] nvarchar(255) NOT NULL,
  [DIEM_DAU] int NOT NULL, 
  [DIEM_CUOI] int NOT NULL,
  [KHOANG_CACH] float NOT NULL,
  [THOI_GIAN_DUY_CHUYEN] float NOT NULL,
  [TINH_TRANG] varchar(128) NOT NULL CHECK ([TINH_TRANG] IN ('CON_HOAT_DONG', 'KHONG_HOAT_DONG')) DEFAULT 'CON_HOAT_DONG'
)
GO


CREATE TABLE [Xe] (
  [ID_XE] INT IDENTITY(1,1) PRIMARY KEY,
  [BIEN_SO_XE] varchar(128) UNIQUE NOT NULL,
  [LOAI_XE] varchar(128) NOT NULL CHECK ([LOAI_XE] IN ('Ghe', 'Limousine', 'Giuong')) ,
  [SO_GHE] int NOT NULL,
  [NGAY_THEM_XE] datetime NOT NULL DEFAULT GETDATE()
)
GO



CREATE TABLE [Ghe] (
  [ID_GHE] VARCHAR(128) PRIMARY KEY,
  [VI_TRI_NGOI] varchar(128) NOT NULL,
  [MA_LICH_TRINH] VARCHAR(128) NOT NULL,
  [TINH_TRANG] BIT NOT NULL DEFAULT 0
)
GO

CREATE TABLE [userAccount] (
  [username] varchar(128) PRIMARY KEY,
  [password] varchar(128) NOT NULL,
  [role] varchar(128) NOT NULL CHECK ([role] IN ('khach','nha_xe')) 
)
GO

CREATE TABLE NHANVIEN(
	[USERNAME] varchar(128) PRIMARY KEY,
	[TEN_NHANVIEN] nvarchar(255) NOT NULL,
	[SDT] varchar(15) UNIQUE NOT NULL,
	[EMAIL] varchar(128) NOT NULL,
	[LOAI_NV] NVARCHAR(32) NOT NULL CHECK ([LOAI_NV] IN ('QUAN_LY', 'TAI_XE'))
)
GO

CREATE TABLE [LichTrinh] (
  [MA_LICH_TRINH] varchar(128) PRIMARY KEY,
  [ID_TUYEN_DUONG] INT NOT NULL,
  [KHOI_HANH] datetime NOT NULL,
  [KET_THUC] datetime NOT NULL,
  [GIA_VE] float NOT NULL,
  [ID_XE] INT NOT NULL,
  [CHI_PHI_PHAT_SINH] float,
  [TAIXE] varchar(128) NOT NULL foreign key references NHANVIEN([USERNAME]),
  [NGAY_TAO_LICH_TRINH]  datetime NOT NULL DEFAULT GETDATE(),
  [NGUOI_TAO_LICH_TRINH] varchar(128) NOT NULL
)
GO

CREATE TABLE [KhachHang] (
  [USERNAME] varchar(128) PRIMARY KEY,
  [TEN_KHACH_HANG] nvarchar(128) NOT NULL,
  [SO_DIEN_THOAI] varchar(128) UNIQUE NOT NULL,
  [EMAIL] varchar(128) NOT NULL
)
GO




CREATE TABLE [Ve] (
  [ID_VE] varchar(128) PRIMARY KEY,
  [ID_KHACH_HANG] varchar(128) NOT NULL,
  [ID_LICH_TRINH] varchar(128) NOT NULL,
  [NGAY_DAT_VE]  datetime NOT NULL DEFAULT GETDATE(),
  [TONG_TIEN] float NOT NULL
 
)
GO

CREATE TABLE [ChiTietVe] (
  [ID_VE] varchar(128) NOT NULL,
  [VI_TRI_NGOI] varchar(128) NOT NULL,
  [QR_CODE] varchar(128) NOT NULL,
  [TRANG_THAI] varchar(128) NOT NULL CHECK ([TRANG_THAI] IN ('da_xac_nhan', 'da_thanh_toan', 'huy_ve')) DEFAULT 'da_thanh_toan',
  primary key(ID_VE,VI_TRI_NGOI)
)
GO

CREATE TABLE [DoanhThu](
	[ID_DOANHTHU] varchar(128) NOT NULL PRIMARY KEY,
	[MA_LICH_TRINH] varchar(128) NOT NULL,
	[SO_VE_DA_DAT]  INT,
	[TONGTIEN] float,
	FOREIGN KEY ([MA_LICH_TRINH]) REFERENCES [LichTrinh] ([MA_LICH_TRINH])
)
Go

CREATE TABLE TramDungChan(
	ID_TRAMDUNGCHAN VARCHAR(128) NOT NULL PRIMARY KEY,
	TEN_TRAMDUNGCHAN VARCHAR(128) NOT NULL,
	ID_DIADIEM int NOT NULL FOREIGN KEY REFERENCES DIADIEM (ID_DIADIEM),
  DIA_CHI nvarchar(258) NOT NULL
)
GO

CREATE TABLE ThemTramDungChan(
	ID_TRAMDUNGCHAN VARCHAR(128) NOT NULL,
	[MA_LICH_TRINH] varchar(128) NOT NULL,
	THOIGIANDEN DATETIME,
  FOREIGN KEY (ID_TRAMDUNGCHAN) REFERENCES TramDungChan (ID_TRAMDUNGCHAN),
	FOREIGN KEY ([MA_LICH_TRINH]) REFERENCES [LichTrinh] ([MA_LICH_TRINH]),
	PRIMARY KEY (ID_TRAMDUNGCHAN,[MA_LICH_TRINH])
)

ALTER TABLE [TuyenDuong] ADD FOREIGN KEY ([DIEM_DAU]) REFERENCES [DiaDiem] ([ID_DIADIEM])
GO

ALTER TABLE [TuyenDuong] ADD FOREIGN KEY ([DIEM_CUOI]) REFERENCES [DiaDiem] ([ID_DIADIEM])
GO


ALTER TABLE [Ghe] ADD FOREIGN KEY ([MA_LICH_TRINH]) REFERENCES [LichTrinh] ([MA_LICH_TRINH])
GO

ALTER TABLE [LichTrinh] ADD FOREIGN KEY ([ID_TUYEN_DUONG]) REFERENCES [TuyenDuong] ([ID_TUYEN])
GO

ALTER TABLE [LichTrinh] ADD FOREIGN KEY ([ID_XE]) REFERENCES [Xe] ([ID_XE])
GO

ALTER TABLE [LichTrinh] ADD FOREIGN KEY ([NGUOI_TAO_LICH_TRINH]) REFERENCES [NHANVIEN] ([USERNAME])
GO

ALTER TABLE [KhachHang] ADD FOREIGN KEY ([USERNAME]) REFERENCES [userAccount] ([username])
GO

ALTER TABLE [NHANVIEN] ADD FOREIGN KEY ([USERNAME]) REFERENCES [userAccount] ([username])
GO

ALTER TABLE [Ve] ADD FOREIGN KEY ([ID_KHACH_HANG]) REFERENCES [KhachHang] ([USERNAME])
GO

ALTER TABLE [Ve] ADD FOREIGN KEY ([ID_LICH_TRINH]) REFERENCES [LichTrinh] ([MA_LICH_TRINH])
GO

ALTER TABLE [ChiTietVe] ADD FOREIGN KEY ([ID_VE]) REFERENCES [Ve] ([ID_VE])
GO

ALTER TABLE [ChiTietVe] ADD FOREIGN KEY ([VI_TRI_NGOI]) REFERENCES [Ghe] ([ID_GHE])
GO
