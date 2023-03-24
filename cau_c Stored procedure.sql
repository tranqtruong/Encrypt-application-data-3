/*---------------------------------------------------------- 
MASV: N18DCAT100, N18DCAT058, N18DCAT102
HO TEN CAC THANH VIEN NHOM: Trần Quốc Trượng, Hồ Minh Phong, Huỳnh Tiến Vĩ
LAB: 04 - NHOM 28
NGAY: 11/3/2021
----------------------------------------------------------*/ 
/*i) Stored dùng để thêm mới dữ liệu (Insert) vào table NHANVIEN, 
trong đó dữ liệu tham số được mã hóa từ client*/
create proc SP_INS_PUBLIC_ENCRYPT_NHANVIEN
	@MANV varchar(20),
	@HOTEN nvarchar(100),
	@EMAIL varchar(20),
	@LUONG	varbinary(MAX),
	@TENDN nvarchar(100),
	@MK	varbinary(MAX),
	@PUB varchar(20)
as
	insert into NHANVIEN
	values(@MANV, @HOTEN, @EMAIL, @LUONG, @TENDN, @MK, @PUB)
go

exec SP_INS_PUBLIC_ENCRYPT_NHANVIEN 'NV00', 'Admin', 'admin@', 0xA961DBD80F575A9A6972A575E98C7A7302CEFD23EF4008854B5EB3EABC02C9CE62917EB92DEA1C388D09E047F2B663A9FA5ED29B59ABAE07F8D6536C8EED35EC, 'admin', 0x8CB2237D0679CA88DB6464EAC60DA96345513964, 'PubKeyAdmin'

select * from NHANVIEN
delete from NHANVIEN
/* ii) Stored dùng để truy vấn dữ liệu nhân viên (NHANVIEN) */
create proc SP_SEL_PUBLIC_ENCRYPT_NHANVIEN
	@TENDN nvarchar(100),
	@MK varbinary(MAX)
as
	select MANV, HOTEN, EMAIL, LUONG, PUBKEY
	from NHANVIEN
	where NHANVIEN.TENDN = @TENDN AND NHANVIEN.MATKHAU = @MK
go

drop proc SP_SEL_PUBLIC_ENCRYPT_NHANVIEN
select * from NHANVIEN

exec SP_SEL_PUBLIC_ENCRYPT_NHANVIEN 'admin', 0x8CB2237D0679CA88DB6464EAC60DA96345513964




