import { getProvinces, getDistricts } from "./api";
// Load provinces
export async function loadProvinces() {
    try {
        const provinces = await getProvinces();
        const provinceSelect = document.getElementById('province');
        
        provinces.forEach(province => {
            const option = new Option(province.name, province.code);
            provinceSelect.add(option);
        });
    } catch (error) {
        console.error('Lỗi khi tải tỉnh/thành:', error);
    }
}
// Handle province change
export async function handleProvinceChange(provinceCode) {
    try {
        const data = await getDistricts(provinceCode);
        const districtSelect = document.getElementById('district');
        
        districtSelect.innerHTML = '<option value="">Chọn quận/huyện</option>';
        document.getElementById('ward').innerHTML = '<option value="">Chọn phường/xã</option>';
        
        data.districts.forEach(district => {
            const option = new Option(district.name, district.code);
            districtSelect.add(option);
        });
    } catch (error) {
        console.error('Lỗi khi tải quận/huyện:', error);
    }
}