// Permission Constants - Danh sách quyền cố định
export const PERMISSIONS = {
  // Hóa đơn
  Hoadon: "30c474e1-38e9-489a-a5b7-d322f0e09123",
  Hoadon_LIST: "30c474e1-38e9-489a-a5b7-d322f0e09c57",

  // Permission
  Permission: "5b32b4ec-c755-415a-98a6-ecae5102d6bd",
  Permission_LIST: "31f88c91-96fa-4ddc-bef7-d7cf71ed2480",
  Permission_ADD: "5a834979-1410-47f1-9bf9-b0429afcb38e",
  Permission_EDIT: "c44601a2-8f2b-4b19-ae23-cc323e2b578e",
  Permission_VIEW: "8c9f1744-1f63-47d9-8c97-82ef3282bf65",
  Permission_IMPORT: "3be24036-48f0-4601-acf5-8700e989cded",
  Permission_EXPORT: "1da6227c-955e-4fc6-ba75-3ef3c1fbc90a",
  Permission_DELETE: "84c7f38d-e9c5-46d0-9a1e-c75462bbdb55",
  Permission_DELETE_TEMP: "f414efac-a569-4df7-8788-7cf7780865ef",
  Permission_RESTORE: "7b2db018-f0c9-4e62-a1fb-0ee99fa13456",
  Permission_HISTORY: "594a7ed6-a738-40bc-ab81-866e890e82ce",

  // Role
  Role: "20cfd7f1-7ee0-468d-91b0-28c937a17812",
  Role_LIST: "66e708f0-00f3-430d-b16d-80d58357c12a",
  Role_ADD: "ffceb425-7223-45ff-9283-8d7c93ad4483",
  Role_EDIT: "15206ba3-9d3f-41e3-b484-90b4b5fdc83d",
  Role_VIEW: "9eb5cd87-3303-4455-bc48-1fd4747bebfe",
  Role_IMPORT: "f6693fa4-9b01-4564-a1af-28cb281cc98d",
  Role_EXPORT: "65154b9c-8dd9-4e8b-9184-7a733ef55a2b",
  Role_DELETE: "28f5e266-cba7-4a75-b145-5cd612641d98",
  Role_DELETE_TEMP: "ff5453bb-8d30-446d-a06e-431a01604313",
  Role_RESTORE: "3f07366f-3048-43f5-b8f1-103664281074",
  Role_HISTORY: "0af7e9e7-8c28-4ef1-b4c4-11ec245e3b5f",

  // Role Permission
  Role_Permission: "7ba31e3a-c7d6-4b77-a26d-2d540c768378",
  Role_Permission_LIST: "c439a659-a054-41b5-87f8-82fc683d62be",
  Role_Permission_ADD: "f4c57c3c-014c-48d9-9eaa-94ec82be1c76",
  Role_Permission_EDIT: "7719d677-ad28-4055-8831-314633797933",
  Role_Permission_VIEW: "9fe142e5-c1c9-4889-baab-f7d6c0ad50c4",
  Role_Permission_IMPORT: "3b8ddb0a-d169-43fe-82d2-3af2b38c9059",
  Role_Permission_EXPORT: "9183e0ac-7a57-4bd6-856f-6a7ca25b2a37",
  Role_Permission_DELETE: "833847c4-b54a-4f67-b6ce-39f7cd1be35b",
  Role_Permission_DELETE_TEMP: "87cf93ec-0d8a-4561-be2b-7edd9e0a272e",
  Role_Permission_RESTORE: "d79c5e49-0fdc-4ea5-9af7-b8620928419a",
  Role_Permission_HISTORY: "ebb39f02-3d2c-4b46-b14f-ac3395b9f0d8",

  // Thanh toán
  Thanhtoan: "ad5126b2-a617-400b-868f-4ffda4e255ec",
  Thanhtoan_LIST: "30de040b-9c9f-4786-b954-309469428413",

  // User
  User: "ebb7d7f0-ca3a-4dfe-be48-7182bf139123",
  User_LIST: "ebb7d7f0-ca3a-4dfe-be48-7182bf139152",
}

// Route permission mapping - Ánh xạ route với quyền cần thiết
export const ROUTE_PERMISSIONS = {
  '/hoa-don': [PERMISSIONS.Hoadon, PERMISSIONS.Hoadon_LIST],
  '/nguoi-dung': [PERMISSIONS.User, PERMISSIONS.User_LIST],
  '/role': [PERMISSIONS.Role, PERMISSIONS.Role_LIST],
  '/permission': [PERMISSIONS.Permission, PERMISSIONS.Permission_LIST],
  '/role-permission': [PERMISSIONS.Role_Permission, PERMISSIONS.Role_Permission_LIST],
  '/thanh-toan': [PERMISSIONS.Thanhtoan, PERMISSIONS.Thanhtoan_LIST],
}

// Feature permission mapping - Ánh xạ chức năng với quyền
export const FEATURE_PERMISSIONS = {
  // Permission features
  permission: {
    list: PERMISSIONS.Permission_LIST,
    add: PERMISSIONS.Permission_ADD,
    edit: PERMISSIONS.Permission_EDIT,
    view: PERMISSIONS.Permission_VIEW,
    delete: PERMISSIONS.Permission_DELETE,
    deleteTemp: PERMISSIONS.Permission_DELETE_TEMP,
    restore: PERMISSIONS.Permission_RESTORE,
    import: PERMISSIONS.Permission_IMPORT,
    export: PERMISSIONS.Permission_EXPORT,
    history: PERMISSIONS.Permission_HISTORY,
  },
  // Role features
  role: {
    list: PERMISSIONS.Role_LIST,
    add: PERMISSIONS.Role_ADD,
    edit: PERMISSIONS.Role_EDIT,
    view: PERMISSIONS.Role_VIEW,
    delete: PERMISSIONS.Role_DELETE,
    deleteTemp: PERMISSIONS.Role_DELETE_TEMP,
    restore: PERMISSIONS.Role_RESTORE,
    import: PERMISSIONS.Role_IMPORT,
    export: PERMISSIONS.Role_EXPORT,
    history: PERMISSIONS.Role_HISTORY,
  },
  // Role Permission features
  rolePermission: {
    list: PERMISSIONS.Role_Permission_LIST,
    add: PERMISSIONS.Role_Permission_ADD,
    edit: PERMISSIONS.Role_Permission_EDIT,
    view: PERMISSIONS.Role_Permission_VIEW,
    delete: PERMISSIONS.Role_Permission_DELETE,
    deleteTemp: PERMISSIONS.Role_Permission_DELETE_TEMP,
    restore: PERMISSIONS.Role_Permission_RESTORE,
    import: PERMISSIONS.Role_Permission_IMPORT,
    export: PERMISSIONS.Role_Permission_EXPORT,
    history: PERMISSIONS.Role_Permission_HISTORY,
  },
}

export default PERMISSIONS
