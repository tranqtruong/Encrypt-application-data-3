/*---------------------------------------------------------- 
MASV: N18DCAT100, N18DCAT058, N18DCAT102
HO TEN CAC THANH VIEN NHOM: Trần Quốc Trượng, Hồ Minh Phong, Huỳnh Tiến Vĩ
LAB: 04 - NHOM 28
NGAY: 11/3/2021
----------------------------------------------------------*/ 

--login
create proc Login @Username varchar(20), @Password varbinary(MAX)
as
select * from NHANVIEN where TENDN = @Username AND MATKHAU = @Password
go

--load table nhan vien
create proc LOAD_NHANVIEN
as
	select MANV, HOTEN, EMAIL, LUONG
	from NHANVIEN
go

--sửa 1 nhân viên
create proc UPDATE_NHANVIEN 
	@MANV varchar(20),
	@HOTEN nvarchar(100),
	@EMAIL varchar(20),
	@LUONG	varbinary(MAX),
	@TENDN nvarchar(100),
	@MK	varbinary(MAX)
as
	update NHANVIEN
	set HOTEN = @HOTEN, EMAIL = @EMAIL, LUONG = @LUONG, TENDN = @TENDN, MATKHAU = @MK
	where MANV = @MANV;
go

-- xóa nhân viên
create proc DELETE_NHANVIEN @MANV varchar(20)
as
	delete from NHANVIEN where MANV = @MANV;
go


--load table LOP
create proc LOAD_LOPHOC
as
	select * from LOP
go

-- thêm lớp học
create proc INSERT_LOPHOC
	@MALOP varchar(20),
	@TENLOP nvarchar(100),
	@MANV varchar(20)
as
	insert into LOP
	values(@MALOP, @TENLOP, @MANV)
go

-- xóa lớp học
create proc DELETE_LOPHOC @MALOP varchar(20)
as
	delete from LOP where MALOP = @MALOP;
go

-- sửa thông tin lớp học
create proc MODIFY_LOPHOC
	@MALOP varchar(20),
	@TENLOP nvarchar(100),
	@MANV varchar(20)
as
	update LOP
	set TENLOP = @TENLOP, MANV = @MANV
	where MALOP = @MALOP;
go




-- load table SINHVIEN
create proc LOAD_ALL_SINHVIEN
as
	select * from SINHVIEN
go

create proc LOAD_SINHVIEN @MALOP varchar(20)
as
	select * from SINHVIEN
	where MALOP = @MALOP;
go

-- thêm sinh viên
create proc INSERT_SV
	@MASV varchar(20),
	@HOTEN nvarchar(100),
	@NGAYSINH datetime,
	@DIACHI nvarchar(200),
	@MALOP varchar(20),
	@TENDN nvarchar(100),
	@MATKHAU varbinary(MAX)
as
	insert into SINHVIEN
	values(@MASV, @HOTEN, @NGAYSINH, @DIACHI, @MALOP, @TENDN, @MATKHAU)
go

-- xóa sinh viên
create proc DELETE_SINHVIEN @MASV varchar(20)
as
	delete from SINHVIEN where MASV = @MASV;
go

-- sửa thông tin sinh viên
create proc MODIFY_SINHVIEN
	@MASV varchar(20),
	@HOTEN nvarchar(100),
	@NGAYSINH datetime,
	@DIACHI nvarchar(200),
	@MALOP varchar(20),
	@TENDN nvarchar(100),
	@MATKHAU varbinary(MAX)
as
	update SINHVIEN
	set HOTEN = @HOTEN, NGAYSINH = @NGAYSINH, DIACHI = @DIACHI, MALOP = @MALOP, TENDN = @TENDN, MATKHAU = @MATKHAU
	where MASV = @MASV;
go





--load table học phần
create proc LOAD_HOCPHAN
as
select * from HOCPHAN
go
--insert học phần
create proc INSERT_HOCPHAN
	@MAHP varchar(20),
	@TENHP nvarchar(100),
	@SOTC int
as
	insert into HOCPHAN
	values(@MAHP, @TENHP, @SOTC)
go
-- sửa học phần
create proc UPDATE_HOCPHAN
	@MAHP varchar(20),
	@TENHP nvarchar(100),
	@SOTC int
as
	update HOCPHAN
	set TENHP = @TENHP, SOTC = @SOTC
	where MAHP = @MAHP;
go
-- xóa học phần
create proc DELETE_HOCPHAN @MAHP varchar(20)
as
	delete from HOCPHAN where MAHP = @MAHP;
go




--load bảng điểm
create proc LOAD_DIEM
as
	select MASV, BANGDIEM.MAHP, TENHP, DIEMTHI
	from BANGDIEM
	inner join HOCPHAN on BANGDIEM.MAHP = HOCPHAN.MAHP;
go
-- insert điểm
create proc INSERT_DIEM
	@MASV varchar(20),
	@MAHP varchar(20),
	@DIEMTHI varbinary(MAX)
as
	insert into BANGDIEM
	values(@MASV, @MAHP, @DIEMTHI);
go
-- sửa điểm
create proc UPDATE_DIEM
	@MASV varchar(20),
	@MAHP varchar(20),
	@DIEMTHI varbinary(MAX)
as
	update BANGDIEM
	set DIEMTHI = @DIEMTHI
	where MASV = @MASV and MAHP = @MAHP;
go
-- xóa điểm
create proc DELETE_DIEM @MASV varchar(20), @MAHP varchar(20)
as
delete from BANGDIEM where MASV = @MASV and MAHP = @MAHP;
go