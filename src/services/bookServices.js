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
  update: (bookUpdateModel) => {
    return axiosClient.put(`/books`, bookUpdateModel);
  },
  getById: (id) => {
    return axiosClient.get(`/books/${id}`);
  },
  create: (bookCreateModel) => axiosClient.post(`/books`, bookCreateModel),
};
