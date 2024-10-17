
USE [nhaxe_quocvuong]
GO
/****** Object:  Trigger [dbo].[trgSetSoGhe]    Script Date: 12/09/2024 1:01:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[trgSetSoGhe]
ON [dbo].[LichTrinh]
AFTER INSERT, UPDATE
AS
BEGIN

   -- XOA TRC CAC GHE DA TON TAI TRONG TRUONG HOP UPDATE
    IF EXISTS (SELECT 1 FROM Ghe WHERE Ghe.MA_LICH_TRINH IN (SELECT MA_LICH_TRINH FROM inserted))
	BEGIN
			DELETE FROM Ghe
			WHERE MA_LICH_TRINH IN (SELECT MA_LICH_TRINH FROM inserted);
	END

	-- INSERT SỐ LƯỢNG GHẾ ZO TÙY THEO CASE TRÊN
	INSERT INTO Ghe (ID_GHE,VI_TRI_NGOI,MA_LICH_TRINH)
	SELECT CONCAT(MA_LICH_TRINH,' ',BIEN_SO_XE, '_', GroupLetter, SeatNumber) AS ID_GHE,
		   CONCAT(GroupLetter, SeatNumber) AS VI_TRI_NGOI,
		   MA_LICH_TRINH FROM 
		   (SELECT 
			X.ID_XE,
			X.BIEN_SO_XE,
			X.LOAI_XE,
			X.SO_GHE,
			LT.MA_LICH_TRINH,
			N.Number,
			CASE 
				WHEN N.Number <= CASE 
					   WHEN X.LOAI_XE = 'Ghe' THEN 12
					   WHEN X.LOAI_XE = 'Limousine' THEN 24
					   WHEN X.LOAI_XE = 'Giuong' THEN 36
			   ELSE 0 
			   END  / 2 THEN 'A'
				ELSE 'B'
			END AS GroupLetter,
			ROW_NUMBER() OVER (PARTITION BY X.ID_XE, 
				CASE 
					WHEN N.Number <= CASE 
					   WHEN X.LOAI_XE = 'Ghe' THEN 12
					   WHEN X.LOAI_XE = 'Limousine' THEN 24
					   WHEN X.LOAI_XE = 'Giuong' THEN 36
			   ELSE 0 
			   END  / 2 THEN 'A'
				ELSE 'B'
				END
				ORDER BY N.Number
			) AS SeatNumber
		FROM 
			inserted LT join dbo.Xe X ON X.ID_XE=LT.ID_XE
			CROSS JOIN ( SELECT TOP 36 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS Number
		FROM sys.objects) N
		WHERE 
			N.Number <= CASE 
					   WHEN X.LOAI_XE = 'Ghe' THEN 12
					   WHEN X.LOAI_XE = 'Limousine' THEN 24
					   WHEN X.LOAI_XE = 'Giuong' THEN 36
			   ELSE 0 
			   END ) as NumberedSeats
		--ORDER BY ID_XE, GroupLetter, SeatNumber
END





-- Thêm dữ liệu vào bảng DiaDiem
INSERT INTO [DiaDiem] ([TEN_TINH_THANH]) VALUES 
(N'Hà Nội'),
(N'Hải Phòng'),
(N'Đà Nẵng'),
(N'Hồ Chí Minh'),
(N'Cần Thơ');

-- Thêm dữ liệu vào bảng TuyenDuong
INSERT INTO [TuyenDuong] ([TEN_TUYEN], [DIEM_DAU], [DIEM_CUOI], [KHOANG_CACH], [THOI_GIAN_DUY_CHUYEN], [TINH_TRANG]) VALUES 
(N'Hà Nội - Hải Phòng', 1, 2, 120, 2, 'CON_HOAT_DONG'),
(N'Đà Nẵng - Hồ Chí Minh', 3, 4, 850, 16, 'CON_HOAT_DONG'),
(N'Hà Nội - Đà Nẵng', 1, 3, 760, 14, 'KHONG_HOAT_DONG'),
(N'Hồ Chí Minh - Cần Thơ', 4, 5, 180, 4, 'CON_HOAT_DONG'),
(N'Hải Phòng - Cần Thơ', 2, 5, 1500, 30, 'CON_HOAT_DONG');

-- Thêm dữ liệu vào bảng Xe
INSERT INTO [Xe] ([BIEN_SO_XE], [LOAI_XE], [SO_GHE]) VALUES 
('29A-12345', 'Ghe', 12),
('30B-67890', 'Limousine', 24),
('31C-11111', 'Giuong', 36),
('32D-22222', 'Ghe', 12),
('33E-33333', 'Limousine', 24);



-- Thêm dữ liệu vào bảng userAccount
INSERT INTO [userAccount] ([username], [password], [role]) VALUES 
('khach01', 'pass123', 'khach'),
('khach02', 'pass234', 'khach'),
('nha_xe01', 'pass345', 'nha_xe'),
('nha_xe02', 'pass456', 'nha_xe'),
('admin01', 'adminpass', 'khach');

select * from [userAccount]

-- Thêm dữ liệu vào bảng NHANVIEN
INSERT INTO [NHANVIEN] ([USERNAME], [TEN_NHANVIEN], [SDT], [EMAIL], [LOAI_NV]) VALUES 
('nha_xe01', N'Nguyễn Văn A', '0123456789', 'a@example.com', 'QUAN_LY'),
('nha_xe02', N'Trần Văn B', '0987654321', 'b@example.com', 'TAI_XE');

-- Thêm dữ liệu vào bảng LichTrinh
INSERT INTO [LichTrinh] ([MA_LICH_TRINH], [ID_TUYEN_DUONG], [KHOI_HANH], [KET_THUC], [GIA_VE], [ID_XE], [CHI_PHI_PHAT_SINH], [TAIXE], [NGUOI_TAO_LICH_TRINH]) VALUES 
('LT001', 1, '2024-10-20 08:00:00', '2024-10-20 10:00:00', 200000, 1, 0, 'nha_xe02', 'nha_xe01');
INSERT INTO [LichTrinh] ([MA_LICH_TRINH], [ID_TUYEN_DUONG], [KHOI_HANH], [KET_THUC], [GIA_VE], [ID_XE], [CHI_PHI_PHAT_SINH], [TAIXE], [NGUOI_TAO_LICH_TRINH]) VALUES 
('LT002', 2, '2024-10-21 06:00:00', '2024-10-21 22:00:00', 1200000, 2, 50000, 'nha_xe02', 'nha_xe01');
INSERT INTO [LichTrinh] ([MA_LICH_TRINH], [ID_TUYEN_DUONG], [KHOI_HANH], [KET_THUC], [GIA_VE], [ID_XE], [CHI_PHI_PHAT_SINH], [TAIXE], [NGUOI_TAO_LICH_TRINH]) VALUES 
('LT003', 3, '2024-10-22 07:00:00', '2024-10-22 21:00:00', 800000, 3, 30000, 'nha_xe02', 'nha_xe01'),
('LT004', 4, '2024-10-23 09:00:00', '2024-10-23 13:00:00', 150000, 4, 0, 'nha_xe02', 'nha_xe01'),
('LT005', 5, '2024-10-24 05:00:00', '2024-10-25 11:00:00', 1700000, 5, 100000, 'nha_xe02', 'nha_xe01');

-- Thêm dữ liệu vào bảng KhachHang
INSERT INTO [KhachHang] ([USERNAME], [TEN_KHACH_HANG], [SO_DIEN_THOAI], [EMAIL]) VALUES 
('khach01', N'Trần Thị Mai', '0987000001', 'mai@example.com'),
('khach02', N'Nguyễn Văn Hùng', '0987000002', 'hung@example.com');

-- Thêm dữ liệu vào bảng Ve
INSERT INTO [Ve] ([ID_VE], [ID_KHACH_HANG], [ID_LICH_TRINH], [NGAY_DAT_VE], [TONG_TIEN]) VALUES 
('VE001', 'khach01', 'LT001', '2024-10-18 09:00:00', 200000);
INSERT INTO [Ve] ([ID_VE], [ID_KHACH_HANG], [ID_LICH_TRINH], [NGAY_DAT_VE], [TONG_TIEN]) VALUES 
('VE002', 'khach02', 'LT002', '2024-10-18 10:00:00', 1200000);
INSERT INTO [Ve] ([ID_VE], [ID_KHACH_HANG], [ID_LICH_TRINH], [NGAY_DAT_VE], [TONG_TIEN]) VALUES 
('VE003', 'khach02', 'LT003', '2024-10-18 11:00:00', 800000);
INSERT INTO [Ve] ([ID_VE], [ID_KHACH_HANG], [ID_LICH_TRINH], [NGAY_DAT_VE], [TONG_TIEN]) VALUES 
('VE004', 'khach02', 'LT004', '2024-10-18 12:00:00', 150000);
INSERT INTO [Ve] ([ID_VE], [ID_KHACH_HANG], [ID_LICH_TRINH], [NGAY_DAT_VE], [TONG_TIEN]) VALUES 
('VE005', 'khach01', 'LT005', '2024-10-18 13:00:00', 1700000);


-- Thêm dữ liệu vào bảng ChiTietVe
INSERT INTO [ChiTietVe] ([ID_VE], [VI_TRI_NGOI], [QR_CODE], [TRANG_THAI]) VALUES 
('VE001', '1A', 'QR1A', 'da_thanh_toan');
INSERT INTO [ChiTietVe] ([ID_VE], [VI_TRI_NGOI], [QR_CODE], [TRANG_THAI]) VALUES 
('VE002', '2A', 'QR2A', 'da_thanh_toan');
INSERT INTO [ChiTietVe] ([ID_VE], [VI_TRI_NGOI], [QR_CODE], [TRANG_THAI]) VALUES 
('VE003', '3A', 'QR3A', 'da_xac_nhan');
INSERT INTO [ChiTietVe] ([ID_VE], [VI_TRI_NGOI], [QR_CODE], [TRANG_THAI]) VALUES 
('VE004', '4A', 'QR4A', 'huy_ve');
INSERT INTO [ChiTietVe] ([ID_VE], [VI_TRI_NGOI], [QR_CODE], [TRANG_THAI]) VALUES 
('VE005', '5A', 'QR5A', 'da_thanh_toan');


-- Thêm dữ liệu vào bảng DoanhThu
INSERT INTO [DoanhThu] ([ID_DOANHTHU], [MA_LICH_TRINH], [SO_VE_DA_DAT], [TONGTIEN]) VALUES 
('DT001', 'LT001', 10, 2000000),
('DT002', 'LT002', 8, 9600000),
('DT003', 'LT003', 6, 4800000),
('DT004', 'LT004', 12, 1800000),
('DT005', 'LT005', 4, 6800000);

-- Thêm dữ liệu vào bảng TramDungChan
INSERT INTO [TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES 
('TDC001', N'Trạm dừng chân Hà Nội', 1, N'Số 1, Đường ABC, Hà Nội'),
('TDC002', N'Trạm dừng chân Hải Phòng', 2, N'Số 2, Đường XYZ, Hải Phòng'),
('TDC003', N'Trạm dừng chân Đà Nẵng', 3, N'Số 3, Đường DEF, Đà Nẵng'),
('TDC004', N'Trạm dừng chân Hồ Chí Minh', 4, N'Số 4, Đường GHI, Hồ Chí Minh'),
('TDC005', N'Trạm dừng chân Cần Thơ', 5, N'Số 5, Đường JKL, Cần Thơ');

-- Thêm dữ liệu vào bảng ThemTramDungChan
INSERT INTO [ThemTramDungChan] ([ID_TRAMDUNGCHAN], [MA_LICH_TRINH], [THOIGIANDEN]) VALUES 
('TDC001', 'LT001', '2024-10-20 08:30:00'),
('TDC002', 'LT001', '2024-10-20 09:30:00'),
('TDC003', 'LT002', '2024-10-21 10:30:00'),
('TDC004', 'LT004', '2024-10-23 12:00:00'),
('TDC005', 'LT005', '2024-10-24 07:00:00');

