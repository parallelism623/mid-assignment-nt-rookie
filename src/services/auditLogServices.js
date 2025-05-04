import axiosClient from "../config/axiosConfig";

export const auditLogServices = {
  getUserAuditLog: (id, queryParameters) =>
    axiosClient.get(`users/${id}/audit-logs`, { params: queryParameters }),
};
