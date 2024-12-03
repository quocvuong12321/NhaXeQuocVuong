
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

-- Thêm dữ liệu vào bảng DiaDiem
INSERT INTO [DiaDiem] ([TEN_TINH_THANH]) VALUES 
(N'Hồ Chí Minh'),
(N'Đắk Lắk'),
(N'oà Lạt')
go

-- Thêm dữ liệu vào bảng TuyenDuong
INSERT INTO [TuyenDuong] ([TEN_TUYEN], [DIEM_DAU], [DIEM_CUOI], [KHOANG_CACH], [THOI_GIAN_DUY_CHUYEN], [TINH_TRANG]) VALUES 
(N'Hồ Chí Mình - Đăk Lăk', 1, 2, 350, 8, 'CON_HOAT_DONG'),
(N'Đăk Lăk - Hồ Chí Mình', 2, 1, 350, 8, 'CON_HOAT_DONG'),
(N'Hồ Chí Mình - Đà Lạt', 1, 3, 300, 7, 'CON_HOAT_DONG'),
(N'Đà Lạt - Hồ Chí Minh', 3, 1, 300, 7, 'CON_HOAT_DONG'),
(N'Đăk Lăk - Đà Lạt', 2, 3, 200, 5, 'KHONG_HOAT_DONG');
go
-- Thêm dữ liệu vào bảng Xe
INSERT INTO [Xe] ([BIEN_SO_XE], [LOAI_XE], [SO_GHE]) VALUES 
('30B-67890', 'Limousine', 24),
('31C-11111', 'Giuong', 36),
('33E-33333', 'Limousine', 24);
go


-- Thêm dữ liệu vào bảng userAccount
INSERT INTO [userAccount] ([username], [password], [role]) VALUES 
('khach01', 'pass123', 'khach'),
('khach02', 'pass234', 'khach'),
('nha_xe01', 'pass345', 'nha_xe'),
('nha_xe02', 'pass456', 'nha_xe'),
('admin01', 'adminpass', 'khach');
go
-- Thêm dữ liệu vào bảng NHANVIEN
INSERT INTO [NHANVIEN] ([USERNAME], [TEN_NHANVIEN], [SDT], [EMAIL], [LOAI_NV]) VALUES 
('nha_xe01', N'Nguyễn Văn A', '0123456789', 'a@example.com', 'QUAN_LY'),
('nha_xe02', N'Trần Văn B', '0987654321', 'b@example.com', 'TAI_XE');
go


-- Thêm dữ liệu vào bảng KhachHang
INSERT INTO [KhachHang] ([USERNAME], [TEN_KHACH_HANG], [SO_DIEN_THOAI], [EMAIL]) VALUES 
('khach01', N'Trần Thị Mai', '0987000001', 'mai@example.com'),
('khach02', N'Nguyễn Văn Hùng', '0987000002', 'hung@example.com');
go

INSERT INTO PHUONG_THUC_THANH_TOAN values
('PT1',N'Tiền mặt',''),
('PT2',N'Momo',''),
('PT3',N'Thẻ tín dụng','')
go

-- Thêm dữ liệu vào bảng TramDungChan
INSERT INTO [TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES 
 ('TDC001', N'Bến xe Miền Đông', 1, N'292 Đinh Bộ Lĩnh, Bình Thạnh');
INSERT INTO [TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES 
 ('TDC002', N'Bến xe An Sương', 1, N'Quốc lộ 22, Bà Điểm, Hóc Môn');
INSERT INTO [TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES 
 ('TDC003', N'Mũi Tàu Trường Trinh', 1, N'19B1 Trường Chinh, Tân Bình');
INSERT INTO [TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES 
 ('TDC004', N'Bến xe Miền Tây', 1, N'395 Kinh Dương Vương, Bình Tân');
INSERT INTO [TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES 
 ('TDC005', N'Coop Mart Bảo Lộc', 3, N'Tháp Nước, đường Trần Phú, Phường 2, Bảo Lộc, Lâm Đồng');
INSERT INTO [TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES 
 ('TDC006', N'Bến xe Di Linh', 3, N'681 Hùng Vương, Di Linh, Lâm Đồng');
INSERT INTO [TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES 
 ('TDC007', N'Bến xe phía Nam', 2, N'Võ Văn Kiệt, Khánh Xuân, Buôn Ma Thuột, Đắk Lắk');
INSERT INTO [TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES 
 ('TDC008', N'Ngã 3 Duy Hoà', 2, N'387 Võ Văn Kiệt, Phường Khánh Xuân, Buôn Ma Thuột, Đắk Lắk');
INSERT INTO [TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES 
 ('TDC009', N'Bến xe phía Bắc', 2, N'71 Nguyễn Chí Thanh, Tân An, Buôn Ma Thuột, Đắk Lắk');
INSERT INTO [TramDungChan] ([ID_TRAMDUNGCHAN], [TEN_TRAMDUNGCHAN], [ID_DIADIEM], [DIA_CHI]) VALUES 
 ('TDC011', N'Bến xe liên tỉnh Đà Lạt', 3, N'01 Tô Hiến Thành - P3 - Đà Lạt - Lâm Đồng');
go

