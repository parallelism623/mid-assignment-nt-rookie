import axiosClient from "../config/axiosConfig";

export const reportServices = {
  getBookBorrowingReport: (reportQueryParameters) =>
    axiosClient.get("/reports/book-borrowings", {
      params: reportQueryParameters,
    }),
  getCategoryReport: (reportQueryParameters) =>
    axiosClient.get("/reports/categories", { params: reportQueryParameters }),
  getUserEngagementReport: (reportQueryParameters) =>
    axiosClient.get("/reports/users", { params: reportQueryParameters }),
};
