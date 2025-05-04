import React, { useEffect, useState } from "react";
import { reportServices } from "../../services/reportServices";
import ReportTable from "./ReportTable";

const CategoryReport = ({ reportQueryParameters }) => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    let isSetData = true;
    setLoading(true);

    reportServices
      .getCategoryReport(reportQueryParameters)
      .then((res) => {
        if (isSetData) {
          setData(res.items);
          setLoading(false);
        }
      })
      .catch(() => {
        setLoading(false);
      });

    return () => {
      isSetData = false;
    };
  }, [
    reportQueryParameters.toDate,
    reportQueryParameters.fromDate,
    reportQueryParameters.top,
  ]);

  const columns = [
    {
      title: "Category Name",
      dataIndex: "name",
      key: "name",
    },
    {
      title: "Total Books",
      dataIndex: "totalBook",
      key: "totalBook",
    },
    {
      title: "Quantity Books",
      dataIndex: "quantityBook",
      key: "quantityBook",
    },
    {
      title: "Available Books",
      dataIndex: "availableBook",
      key: "availableBook",
    },
    {
      title: "Total Borrow Requests",
      dataIndex: "totalBorrowRequest",
      key: "totalBorrowRequest",
    },
    {
      title: "Most Requested Book",
      dataIndex: "mostRequestedBook",
      key: "mostRequestedBook",
    },
    {
      title: "Request Count",
      dataIndex: "requestCount",
      key: "requestCount",
    },
  ];

  return (
    <ReportTable
      title={`📚 Report Top ${reportQueryParameters.top} Book Categories from ${reportQueryParameters.fromDate} to ${reportQueryParameters.toDate}`}
      columns={columns}
      loading={loading}
      data={data}
    />
  );
};

export default CategoryReport;
