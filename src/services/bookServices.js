import axiosClient from "../config/axiosConfig";
import qs from "qs";
export const bookServices = {
  gets: (queryParams) => {
    return axiosClient.get("/books", {
      params: queryParams,
      paramsSerializer: (params) => {
        return qs.stringify(params);
      },
    });
  },
  deleteById: (id) => axiosClient.delete(`/books/${id}`),
  update: (formData) => {
    return axiosClient.put(`/books`, formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
  },
  getById: (id) => {
    return axiosClient.get(`/books/${id}`);
  },
  create: (formData) =>
    axiosClient.post(`/books`, formData, {
      headers: { "Content-Type": "multipart/form-data" },
    }),
};
