import axiosClient from "../config/axiosConfig";

export const userServices = {
  getById: (id) => axiosClient.get(`/users/${id}`),
  createBookBorrowingRequest: (bookBorrowingRequest) =>
    axiosClient.post(`/users/book-borrowing`, bookBorrowingRequest),
  getBookBorrowingRequests: (queryParameters, id) =>
    axiosClient.get(`/users/${id}/book-borrowing-requests`, {
      params: queryParameters,
    }),
  getBookBorrowed: (id, queryParameters) =>
    axiosClient.get(`/users/${id}/book-borrowed`, { params: queryParameters }),
  extendDueDate: (id, requestModel) =>
    axiosClient.put(`/users/${id}/book-borrowed/due-date-extend`, requestModel),
  get: (queryParameters) =>
    axiosClient.get("/users", { params: queryParameters }),
  update: (updateRequest) => axiosClient.put("/users", updateRequest),
  create: (createRequest) => axiosClient.post("/users", createRequest),
  delete: (id) => axiosClient.delete(`/users/${id}`),
  updateProfile: (id, request) =>
    axiosClient.put(`/users/${id}/profile`, request),
};
