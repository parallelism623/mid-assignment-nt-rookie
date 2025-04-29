import axiosClient from "../config/axiosConfig";

export const booksBorrowingRequestDetailServices = {
  getsAsync: (queryParameters) =>
    axiosClient.get("/book-borrowing-request-details", {
      params: queryParameters,
    }),
  adjustExtendDueDateRequest: (id, requestModel) =>
    axiosClient.put(
      `/book-borrowing-request-details/${id}/due-date-extend`,
      requestModel
    ),
};
