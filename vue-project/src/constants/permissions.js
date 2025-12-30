// Permission Constants - Danh sách quyền cố định
export const PERMISSIONS = {
  // Hóa đơn
  HOADON: "30c474e1-38e9-489a-a5b7-d322f0e09c11",
  HOADON_LIST: "30c474e1-38e9-489a-a5b7-d322f0e09c57",
  HOADON_ADD: "30c474e1-38e9-489a-a5b7-d322f0e09c58",
  HOADON_EDIT: "30c474e1-38e9-489a-a5b7-d322f0e09c59",
  HOADON_VIEW: "30c474e1-38e9-489a-a5b7-d322f0e09c60",
  HOADON_DELETE: "30c474e1-38e9-489a-a5b7-d322f0e09c61",

  // User
  USER: "ebb7d7f0-ca3a-4dfe-be48-7182bf139111",
  USER_LIST: "ebb7d7f0-ca3a-4dfe-be48-7182bf139152",
  USER_ADD: "ebb7d7f0-ca3a-4dfe-be48-7182bf139153",
  USER_EDIT: "ebb7d7f0-ca3a-4dfe-be48-7182bf139154",
  USER_VIEW: "ebb7d7f0-ca3a-4dfe-be48-7182bf139155",
  USER_DELETE: "ebb7d7f0-ca3a-4dfe-be48-7182bf139156",

  // Role
  ROLE: "66e708f0-00f3-430d-b16d-80d58357c133",
  ROLE_LIST: "66e708f0-00f3-430d-b16d-80d58357c12a",
  ROLE_ADD: "ffceb425-7223-45ff-9283-8d7c93ad4483",
  ROLE_EDIT: "15206ba3-9d3f-41e3-b484-90b4b5fdc83d",
  ROLE_VIEW: "9eb5cd87-3303-4455-bc48-1fd4747bebfe",
  ROLE_DELETE: "28f5e266-cba7-4a75-b145-5cd612641d98",

  // Permission
  PERMISSION_LIST: "31f88c91-96fa-4ddc-bef7-d7cf71ed2480",

  // Role Permission
  ROLEPERMISSION: "c439a659-a054-41b5-87f8-82fc683d6211",
  ROLEPERMISSION_LIST: "c439a659-a054-41b5-87f8-82fc683d62be",
  ROLEPERMISSION_ADD: "f4c57c3c-014c-48d9-9eaa-94ec82be1c76",
  ROLEPERMISSION_EDIT: "7719d677-ad28-4055-8831-314633797933",
  ROLEPERMISSION_VIEW: "9fe142e5-c1c9-4889-baab-f7d6c0ad50c4",
  ROLEPERMISSION_DELETE: "833847c4-b54a-4f67-b6ce-39f7cd1be35b",

  // Order/Thuchi
  ORDER_THUCHI: "ad5126b2-a617-400b-868f-4ffda4e255ec",

  // Thống kê
  THONGKE: "ad5126b2-a617-400b-868f-4ffda4e25890",
}

// Route permission mapping - Ánh xạ route với quyền cần thiết
export const ROUTE_PERMISSIONS = {
  '/hoa-don': [PERMISSIONS.HOADON, PERMISSIONS.HOADON_LIST],
  '/nguoi-dung': [PERMISSIONS.USER, PERMISSIONS.USER_LIST],
  '/role': [PERMISSIONS.ROLE, PERMISSIONS.ROLE_LIST],
  '/permission': [PERMISSIONS.PERMISSION_LIST],
  '/role-permission': [PERMISSIONS.ROLEPERMISSION, PERMISSIONS.ROLEPERMISSION_LIST],
  '/order-thuchi': [PERMISSIONS.ORDER_THUCHI],
  '/thong-ke': [PERMISSIONS.THONGKE],
}

// Feature permission mapping - Ánh xạ chức năng với quyền
export const FEATURE_PERMISSIONS = {
  // Hóa đơn features
  hoadon: {
    list: PERMISSIONS.HOADON_LIST,
    add: PERMISSIONS.HOADON_ADD,
    edit: PERMISSIONS.HOADON_EDIT,
    view: PERMISSIONS.HOADON_VIEW,
    delete: PERMISSIONS.HOADON_DELETE,
  },
  // User features
  user: {
    list: PERMISSIONS.USER_LIST,
    add: PERMISSIONS.USER_ADD,
    edit: PERMISSIONS.USER_EDIT,
    view: PERMISSIONS.USER_VIEW,
    delete: PERMISSIONS.USER_DELETE,
  },
  // Role features
  role: {
    list: PERMISSIONS.ROLE_LIST,
    add: PERMISSIONS.ROLE_ADD,
    edit: PERMISSIONS.ROLE_EDIT,
    view: PERMISSIONS.ROLE_VIEW,
    delete: PERMISSIONS.ROLE_DELETE,
  },
  // Permission features
  permission: {
    list: PERMISSIONS.PERMISSION_LIST,
  },
  // Role Permission features
  rolePermission: {
    list: PERMISSIONS.ROLEPERMISSION_LIST,
    add: PERMISSIONS.ROLEPERMISSION_ADD,
    edit: PERMISSIONS.ROLEPERMISSION_EDIT,
    view: PERMISSIONS.ROLEPERMISSION_VIEW,
    delete: PERMISSIONS.ROLEPERMISSION_DELETE,
  },
}

export default PERMISSIONS
