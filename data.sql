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
AFTER INSERT
AS
BEGIN

    IF EXISTS (SELECT 1 FROM Ghe WHERE Ghe.MA_LICH_TRINH IN (SELECT MA_LICH_TRINH FROM inserted))
	BEGIN
			DELETE FROM Ghe
			WHERE MA_LICH_TRINH IN (SELECT MA_LICH_TRINH FROM inserted);
	END

	-- INSERT SỐ LƯỢNG GHẾ VÀO THEO TRƯỜNG HỢP TRÊN
	INSERT INTO Ghe (ID_GHE, VI_TRI_NGOI, MA_LICH_TRINH)
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
		) AS NumberedSeats
END;
GO


CREATE TRIGGER trg_CheckLichTrinhOverlap
ON LichTrinh
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra trùng lặp TAIXE hoặc ID_XE trong khoảng thời gian
    IF EXISTS (
        SELECT 1
        FROM LichTrinh lt
        JOIN inserted i
        ON (
            lt.TAIXE = i.TAIXE OR lt.ID_XE = i.ID_XE
        )
        AND (
            (i.KHOI_HANH BETWEEN lt.KHOI_HANH AND lt.KET_THUC) OR
            (i.KET_THUC BETWEEN lt.KHOI_HANH AND lt.KET_THUC) OR
            (lt.KHOI_HANH BETWEEN i.KHOI_HANH AND i.KET_THUC) OR
            (lt.KET_THUC BETWEEN i.KHOI_HANH AND i.KET_THUC)
        )
    )
    BEGIN
        -- Trả về thông báo lỗi
        RAISERROR ('Trùng lịch trình: Tài xế hoặc xe đã có lịch trong khoảng thời gian này.', 16, 1);
        RETURN;
    END

    -- Nếu không trùng, thêm dữ liệu vào bảng LichTrinh
    INSERT INTO LichTrinh (MA_LICH_TRINH, ID_TUYEN_DUONG, KHOI_HANH, KET_THUC, GIA_VE, ID_XE, CHI_PHI_PHAT_SINH, TRANG_THAI, TAIXE, NGAY_TAO_LICH_TRINH, NGUOI_TAO_LICH_TRINH)
    SELECT MA_LICH_TRINH, ID_TUYEN_DUONG, KHOI_HANH, KET_THUC, GIA_VE, ID_XE, CHI_PHI_PHAT_SINH, TRANG_THAI, TAIXE, NGAY_TAO_LICH_TRINH, NGUOI_TAO_LICH_TRINH
    FROM inserted;
END;
GO






SET IDENTITY_INSERT [dbo].[DiaDiem] ON 

GO
INSERT [dbo].[DiaDiem] ([ID_DIADIEM], [TEN_TINH_THANH]) VALUES (1, N'Hồ Chí Minh')
GO
INSERT [dbo].[DiaDiem] ([ID_DIADIEM], [TEN_TINH_THANH]) VALUES (2, N'Đắk Lắk')
GO
INSERT [dbo].[DiaDiem] ([ID_DIADIEM], [TEN_TINH_THANH]) VALUES (3, N'oà Lạt')
GO
SET IDENTITY_INSERT [dbo].[DiaDiem] OFF
GO
INSERT [dbo].[TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES (N'TDC001', N'Bến xe Miền Đông', 1, N'292 Đinh Bộ Lĩnh, Bình Thạnh')
GO
INSERT [dbo].[TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES (N'TDC002', N'Bến xe An Sương', 1, N'Quốc lộ 22, Bà Điểm, Hóc Môn')
GO
INSERT [dbo].[TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES (N'TDC003', N'Mũi Tàu Trường Trinh', 1, N'19B1 Trường Chinh, Tân Bình')
GO
INSERT [dbo].[TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES (N'TDC004', N'Bến xe Miền Tây', 1, N'395 Kinh Dương Vương, Bình Tân')
GO
INSERT [dbo].[TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES (N'TDC005', N'Coop Mart Bảo Lộc', 3, N'Tháp Nước, đường Trần Phú, Phường 2, Bảo Lộc, Lâm Đồng')
GO
INSERT [dbo].[TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES (N'TDC006', N'Bến xe Di Linh', 3, N'681 Hùng Vương, Di Linh, Lâm Đồng')
GO
INSERT [dbo].[TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES (N'TDC007', N'Bến xe phía Nam', 2, N'Võ Văn Kiệt, Khánh Xuân, Buôn Ma Thuột, Đắk Lắk')
GO
INSERT [dbo].[TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES (N'TDC008', N'Ngã 3 Duy Hoà', 2, N'387 Võ Văn Kiệt, Phường Khánh Xuân, Buôn Ma Thuột, Đắk Lắk')
GO
INSERT [dbo].[TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES (N'TDC009', N'Bến xe phía Bắc', 2, N'71 Nguyễn Chí Thanh, Tân An, Buôn Ma Thuột, Đắk Lắk')
GO
INSERT [dbo].[TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES (N'TDC011', N'Bến xe liên tỉnh Đà Lạt', 3, N'01 Tô Hiến Thành - P3 - Đà Lạt - Lâm Đồng')
GO
SET IDENTITY_INSERT [dbo].[TuyenDuong] ON 

GO
INSERT [dbo].[TuyenDuong] ([ID_TUYEN], [TEN_TUYEN], [DIEM_DAU], [DIEM_CUOI], [KHOANG_CACH], [THOI_GIAN_DUY_CHUYEN], [TINH_TRANG]) VALUES (1, N'Hồ Chí Mình - Đăk Lăk', 1, 2, 350, 8, N'CON_HOAT_DONG')
GO
INSERT [dbo].[TuyenDuong] ([ID_TUYEN], [TEN_TUYEN], [DIEM_DAU], [DIEM_CUOI], [KHOANG_CACH], [THOI_GIAN_DUY_CHUYEN], [TINH_TRANG]) VALUES (2, N'Đăk Lăk - Hồ Chí Mình', 2, 1, 350, 8, N'CON_HOAT_DONG')
GO
INSERT [dbo].[TuyenDuong] ([ID_TUYEN], [TEN_TUYEN], [DIEM_DAU], [DIEM_CUOI], [KHOANG_CACH], [THOI_GIAN_DUY_CHUYEN], [TINH_TRANG]) VALUES (3, N'Hồ Chí Mình - Đà Lạt', 1, 3, 300, 7, N'CON_HOAT_DONG')
GO
INSERT [dbo].[TuyenDuong] ([ID_TUYEN], [TEN_TUYEN], [DIEM_DAU], [DIEM_CUOI], [KHOANG_CACH], [THOI_GIAN_DUY_CHUYEN], [TINH_TRANG]) VALUES (4, N'Đà Lạt - Hồ Chí Minh', 3, 1, 300, 7, N'CON_HOAT_DONG')
GO
INSERT [dbo].[TuyenDuong] ([ID_TUYEN], [TEN_TUYEN], [DIEM_DAU], [DIEM_CUOI], [KHOANG_CACH], [THOI_GIAN_DUY_CHUYEN], [TINH_TRANG]) VALUES (5, N'Đăk Lăk - Đà Lạt', 2, 3, 200, 5, N'KHONG_HOAT_DONG')
GO
SET IDENTITY_INSERT [dbo].[TuyenDuong] OFF
GO
SET IDENTITY_INSERT [dbo].[Xe] ON 

GO
INSERT [dbo].[Xe] ([ID_XE], [BIEN_SO_XE], [LOAI_XE], [SO_GHE], [NGAY_THEM_XE]) VALUES (1, N'30B-67890', N'Limousine', 24, CAST(N'2024-12-05 15:32:01.023' AS DateTime))
GO
INSERT [dbo].[Xe] ([ID_XE], [BIEN_SO_XE], [LOAI_XE], [SO_GHE], [NGAY_THEM_XE]) VALUES (2, N'31C-11111', N'Giuong', 36, CAST(N'2024-12-05 15:32:01.023' AS DateTime))
GO
INSERT [dbo].[Xe] ([ID_XE], [BIEN_SO_XE], [LOAI_XE], [SO_GHE], [NGAY_THEM_XE]) VALUES (3, N'33E-33333', N'Limousine', 24, CAST(N'2024-12-05 15:32:01.023' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Xe] OFF
GO
INSERT [dbo].[userAccount] ([username], [password], [role]) VALUES (N'admin01', N'adminpass', N'khach')
GO
INSERT [dbo].[userAccount] ([username], [password], [role]) VALUES (N'khach01', N'pass123', N'khach')
GO
INSERT [dbo].[userAccount] ([username], [password], [role]) VALUES (N'khach02', N'pass234', N'khach')
GO
INSERT [dbo].[userAccount] ([username], [password], [role]) VALUES (N'nha_xe01', N'pass345', N'nha_xe')
GO
INSERT [dbo].[userAccount] ([username], [password], [role]) VALUES (N'nha_xe02', N'pass456', N'nha_xe')
GO
INSERT [dbo].[NHANVIEN] ([USERNAME], [TEN_NHANVIEN], [SDT], [EMAIL], [LOAI_NV]) VALUES (N'nha_xe01', N'Nguyễn Văn A', N'0123456789', N'a@example.com', N'QUAN_LY')
GO
INSERT [dbo].[NHANVIEN] ([USERNAME], [TEN_NHANVIEN], [SDT], [EMAIL], [LOAI_NV]) VALUES (N'nha_xe02', N'Trần Văn B', N'0987654321', N'b@example.com', N'TAI_XE')
GO
INSERT [dbo].[LichTrinh] ([MA_LICH_TRINH], [ID_TUYEN_DUONG], [KHOI_HANH], [KET_THUC], [GIA_VE], [ID_XE], [CHI_PHI_PHAT_SINH], [TRANG_THAI], [TAIXE], [NGAY_TAO_LICH_TRINH], [NGUOI_TAO_LICH_TRINH]) VALUES (N'LT001', 1, CAST(N'2024-10-20 08:00:00.000' AS DateTime), CAST(N'2024-10-20 10:00:00.000' AS DateTime), 200000, 1, 0, N'MO_BAN', N'nha_xe02', CAST(N'2024-12-05 16:09:43.140' AS DateTime), N'nha_xe01')
GO
INSERT [dbo].[LichTrinh] ([MA_LICH_TRINH], [ID_TUYEN_DUONG], [KHOI_HANH], [KET_THUC], [GIA_VE], [ID_XE], [CHI_PHI_PHAT_SINH], [TRANG_THAI], [TAIXE], [NGAY_TAO_LICH_TRINH], [NGUOI_TAO_LICH_TRINH]) VALUES (N'LT002', 2, CAST(N'2024-10-21 06:00:00.000' AS DateTime), CAST(N'2024-10-21 22:00:00.000' AS DateTime), 1200000, 2, 50000, N'MO_BAN', N'nha_xe02', CAST(N'2024-12-05 16:09:43.157' AS DateTime), N'nha_xe01')
GO
INSERT [dbo].[KhachHang] ([USERNAME], [TEN_KHACH_HANG], [SO_DIEN_THOAI], [EMAIL]) VALUES (N'khach01', N'Trần Thị Mai', N'0987000001', N'mai@example.com')
GO
INSERT [dbo].[KhachHang] ([USERNAME], [TEN_KHACH_HANG], [SO_DIEN_THOAI], [EMAIL]) VALUES (N'khach02', N'Nguyễn Văn Hùng', N'0987000002', N'hung@example.com')
GO
INSERT [dbo].[PHUONG_THUC_THANH_TOAN] ([ID_PTTT], [TEN_PHUONG_THUC_THANH_TOAN], [CACH_THUC_THANH_TOAN]) VALUES (N'PT1', N'Tiền mặt', N'')
GO
INSERT [dbo].[PHUONG_THUC_THANH_TOAN] ([ID_PTTT], [TEN_PHUONG_THUC_THANH_TOAN], [CACH_THUC_THANH_TOAN]) VALUES (N'PT2', N'Momo', N'')
GO
INSERT [dbo].[PHUONG_THUC_THANH_TOAN] ([ID_PTTT], [TEN_PHUONG_THUC_THANH_TOAN], [CACH_THUC_THANH_TOAN]) VALUES (N'PT3', N'Thẻ tín dụng', N'')
GO
INSERT [dbo].[Ve] ([ID_VE], [ID_KHACH_HANG], [ID_LICH_TRINH], [NGAY_DAT_VE], [TONG_TIEN], [DIEM_DOAN], [DIEM_TRA], [QR_CODE], [TRANG_THAI], [PHUONG_THUC_THANH_TOAN]) VALUES (N'LT001VE001', N'khach01', N'LT001', CAST(N'2024-12-05 00:00:00.000' AS DateTime), 400000, N'TDC001', N'TDC007', N'/img/LT001VE001.png', N'da_su_dung', N'PT1')
GO
INSERT [dbo].[Ve] ([ID_VE], [ID_KHACH_HANG], [ID_LICH_TRINH], [NGAY_DAT_VE], [TONG_TIEN], [DIEM_DOAN], [DIEM_TRA], [QR_CODE], [TRANG_THAI], [PHUONG_THUC_THANH_TOAN]) VALUES (N'LT002VE001', N'khach01', N'LT002', CAST(N'2024-12-05 00:00:00.000' AS DateTime), 3600000, N'TDC008', N'TDC002', N'/img/LT002VE001.png', N'da_su_dung', N'PT3')
GO
INSERT [dbo].[ChiTietVe] ([ID_VE], [VI_TRI_NGOI]) VALUES (N'LT001VE001', N'LT001 30B-67890_A1')
GO
INSERT [dbo].[ChiTietVe] ([ID_VE], [VI_TRI_NGOI]) VALUES (N'LT001VE001', N'LT001 30B-67890_B1')
GO
INSERT [dbo].[ChiTietVe] ([ID_VE], [VI_TRI_NGOI]) VALUES (N'LT002VE001', N'LT002 31C-11111_A6')
GO
INSERT [dbo].[ChiTietVe] ([ID_VE], [VI_TRI_NGOI]) VALUES (N'LT002VE001', N'LT002 31C-11111_B4')
GO
INSERT [dbo].[ChiTietVe] ([ID_VE], [VI_TRI_NGOI]) VALUES (N'LT002VE001', N'LT002 31C-11111_B6')
GO


INSERT [dbo].[Ve] ([ID_VE], [ID_KHACH_HANG], [ID_LICH_TRINH], [NGAY_DAT_VE], [TONG_TIEN], [DIEM_DOAN], [DIEM_TRA], [QR_CODE], [TRANG_THAI], [PHUONG_THUC_THANH_TOAN]) VALUES (N'LT002VE002', N'khach01', N'LT002', CAST(N'2024-11-12 00:00:00.000' AS DateTime), 300000, N'TDC008', N'TDC002', N'/img/LT002VE002.png', N'da_su_dung', N'PT3')
GO
INSERT [dbo].[Ve] ([ID_VE], [ID_KHACH_HANG], [ID_LICH_TRINH], [NGAY_DAT_VE], [TONG_TIEN], [DIEM_DOAN], [DIEM_TRA], [QR_CODE], [TRANG_THAI], [PHUONG_THUC_THANH_TOAN]) VALUES (N'LT002VE003', N'khach01', N'LT002', CAST(N'2024-11-13 00:00:00.000' AS DateTime), 320000, N'TDC008', N'TDC002', N'/img/LT002VE003.png', N'da_su_dung', N'PT1')
GO
INSERT [dbo].[Ve] ([ID_VE], [ID_KHACH_HANG], [ID_LICH_TRINH], [NGAY_DAT_VE], [TONG_TIEN], [DIEM_DOAN], [DIEM_TRA], [QR_CODE], [TRANG_THAI], [PHUONG_THUC_THANH_TOAN]) VALUES (N'LT002VE004', N'khach01', N'LT002', CAST(N'2024-11-25 00:00:00.000' AS DateTime), 280000, N'TDC008', N'TDC002', N'/img/LT002VE004.png', N'da_su_dung', N'PT3')
GO
INSERT [dbo].[Ve] ([ID_VE], [ID_KHACH_HANG], [ID_LICH_TRINH], [NGAY_DAT_VE], [TONG_TIEN], [DIEM_DOAN], [DIEM_TRA], [QR_CODE], [TRANG_THAI], [PHUONG_THUC_THANH_TOAN]) VALUES (N'LT002VE005', N'khach01', N'LT002', CAST(N'2024-10-28 00:00:00.000' AS DateTime), 500000, N'TDC008', N'TDC002', N'/img/LT002VE005.png', N'da_su_dung', N'PT2')
GO