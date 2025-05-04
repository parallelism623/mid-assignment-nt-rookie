import axiosClient from "../config/axiosConfig";

export const categoryServices = {
  getById: (id) => axiosClient.get(`categories/${id}`),
  getsName: () => {
    return axiosClient.get("/categories/v2");
  },
  gets: (queryParameters) => {
    return axiosClient.get("/categories", { params: queryParameters });
  },
  delete: (id) => axiosClient.delete(`categories/${id}`),
  update: (requestModel) => axiosClient.put(`categories`, requestModel),
  create: (requestModel) => axiosClient.post(`categories`, requestModel),
};
