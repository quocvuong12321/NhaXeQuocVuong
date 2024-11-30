
USE [nhaxe_quocvuong]
GO



USE [nhaxe_quocvuong]
GO


/****** Object:  Trigger [dbo].[trgSetSoGhe]    Script Date: 12/09/2024 1:01:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER trg_UpdateTinhTrangGhe
on ChiTietVe
AFTER INSERT
AS
BEGIN

	UPDATE GHE
	SET TINH_TRANG = 1
	WHERE ID_GHE = (SELECT VI_TRI_NGOI FROM inserted)
END
go


CREATE TRIGGER [dbo].[trgSetSoGhe]
ON [dbo].[LichTrinh]
AFTER INSERT, UPDATE
AS
BEGIN

    IF EXISTS (SELECT 1 FROM Ghe WHERE Ghe.MA_LICH_TRINH IN (SELECT MA_LICH_TRINH FROM inserted))
	BEGIN
			DELETE FROM Ghe
			WHERE MA_LICH_TRINH IN (SELECT MA_LICH_TRINH FROM inserted);
	END

	-- INSERT SỐ LƯỢNG GHẾ VÀO THEO TRƯỜNG HỢP TRÊN
	INSERT INTO Ghe (ID_GHE, VI_TRI_NGOI, MA_LICH_TRINH,TINH_TRANG)
	SELECT CONCAT(MA_LICH_TRINH, ' ', BIEN_SO_XE, '_', GroupLetter, SeatNumber) AS ID_GHE,
		   CONCAT(GroupLetter, SeatNumber) AS VI_TRI_NGOI,
		   MA_LICH_TRINH 
	FROM 
		(
			SELECT 
				X.ID_XE,
				X.BIEN_SO_XE,
				X.LOAI_XE,
				X.SO_GHE,
				LT.MA_LICH_TRINH,
				N.Number,
				CASE 
					WHEN X.LOAI_XE = 'Limousine' AND N.Number <= 12 THEN 'A'
					WHEN X.LOAI_XE = 'Limousine' AND N.Number > 12 AND N.Number <= 24 THEN 'B'
					WHEN X.LOAI_XE = 'Giuong' AND N.Number <= 12 THEN 'A'
					WHEN X.LOAI_XE = 'Giuong' AND N.Number > 12 AND N.Number <= 24 THEN 'B'
					WHEN X.LOAI_XE = 'Giuong' AND N.Number > 24 AND N.Number <= 36 THEN 'C'
					ELSE NULL
				END AS GroupLetter,
				ROW_NUMBER() OVER (PARTITION BY X.ID_XE, 
					CASE 
						WHEN X.LOAI_XE = 'Limousine' AND N.Number <= 12 THEN 'A'
						WHEN X.LOAI_XE = 'Limousine' AND N.Number > 12 AND N.Number <= 24 THEN 'B'
						WHEN X.LOAI_XE = 'Giuong' AND N.Number <= 12 THEN 'A'
						WHEN X.LOAI_XE = 'Giuong' AND N.Number > 12 AND N.Number <= 24 THEN 'B'
						WHEN X.LOAI_XE = 'Giuong' AND N.Number > 24 AND N.Number <= 36 THEN 'C'
						ELSE NULL
					END
					ORDER BY N.Number
				) AS SeatNumber
			FROM 
				inserted LT 
				JOIN dbo.Xe X ON X.ID_XE = LT.ID_XE
				CROSS JOIN 
				(
					SELECT TOP 36 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS Number
					FROM sys.objects
				) N
			WHERE 
				N.Number <= 
				CASE 
					WHEN X.LOAI_XE = 'Limousine' THEN 24
					WHEN X.LOAI_XE = 'Giuong' THEN 36
					ELSE 0
				END
		) AS NumberedSeats,
END;
go


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
('30B-67890', 'Limousine', 24),
('31C-11111', 'Giuong', 36),
('33E-33333', 'Limousine', 24);



-- Thêm dữ liệu vào bảng userAccount
INSERT INTO [userAccount] ([username], [password], [role]) VALUES 
('khach01', 'pass123', 'khach'),
('khach02', 'pass234', 'khach'),
('nha_xe01', 'pass345', 'nha_xe'),
('nha_xe02', 'pass456', 'nha_xe'),
('admin01', 'adminpass', 'khach');

-- Thêm dữ liệu vào bảng NHANVIEN
INSERT INTO [NHANVIEN] ([USERNAME], [TEN_NHANVIEN], [SDT], [EMAIL], [LOAI_NV]) VALUES 
('nha_xe01', N'Nguyễn Văn A', '0123456789', 'a@example.com', 'QUAN_LY'),
('nha_xe02', N'Trần Văn B', '0987654321', 'b@example.com', 'TAI_XE');






-- Thêm dữ liệu vào bảng KhachHang
INSERT INTO [KhachHang] ([USERNAME], [TEN_KHACH_HANG], [SO_DIEN_THOAI], [EMAIL]) VALUES 
('khach01', N'Trần Thị Mai', '0987000001', 'mai@example.com'),
('khach02', N'Nguyễn Văn Hùng', '0987000002', 'hung@example.com');

INSERT INTO PHUONG_THUC_THANH_TOAN values
('PT1',N'Tiền mặt','abasdasd')


-- Thêm dữ liệu vào bảng TramDungChan
INSERT INTO [TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES 
('TDC001', N'Trạm dừng chân Hà Nội', 1, N'Số 1, Đường ABC, Hà Nội'),
('TDC002', N'Trạm dừng chân Hải Phòng', 2, N'Số 2, Đường XYZ, Hải Phòng'),
('TDC003', N'Trạm dừng chân Đà Nẵng', 3, N'Số 3, Đường DEF, Đà Nẵng'),
('TDC004', N'Trạm dừng chân Hồ Chí Minh', 4, N'Số 4, Đường GHI, Hồ Chí Minh'),
('TDC005', N'Trạm dừng chân Cần Thơ', 5, N'Số 5, Đường JKL, Cần Thơ');






