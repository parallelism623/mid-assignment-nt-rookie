import axiosClient from "../config/axiosConfig";
export const exportServices = {
  exportUserEngagementReport: (queryParameters) =>
    axiosClient.get("/exports/reports/users", {
      params: queryParameters,
      responseType: "arraybuffer",
    }),
  exportCategoriesReport: (queryParameters) =>
    axiosClient.get("/exports/reports/categories", {
      params: queryParameters,
      responseType: "arraybuffer",
    }),
  exportBookBorrowingReport: (queryParameters) =>
    axiosClient.get("/exports/reports/book-borrowings", {
      params: queryParameters,
      responseType: "arraybuffer",
    }),
};
