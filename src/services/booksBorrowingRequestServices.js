import axiosClient from "../config/axiosConfig";
export const booksBorrowingRequestServices = {
  gets: (queryParameters) =>
    axiosClient.get("/book-borrowing-requests", {
      params: queryParameters,
    }),
  getDetail: (id) => axiosClient.get(`/book-borrowing-requests/${id}/detail`),
  changeStatus: (requestModel) =>
    axiosClient.put(`/book-borrowing-requests/status`, requestModel),
};
