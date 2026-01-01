const API_URL = 'https://provinces.open-api.vn/api';
// Lấy danh sách tỉnh/thành từ api
export async function getProvinces() {
    const response = await fetch(`${API_URL}/p`);
    return response.json();
}
// Get list districts from api
export async function getDistricts(provinceCode) {
    const response = await fetch(`${API_URL}/p/${provinceCode}?depth=2`);
    return response.json();
}
// Get list wards from api
export async function getWards(districtCode) {
    const response = await fetch(`${API_URL}/d/${districtCode}?depth=2`);
    return response.json();
}
