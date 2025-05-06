import React, { useEffect, useState } from "react";
import { Table, Typography, Card } from "antd";
import { reportServices } from "../../services/reportServices";
import ReportTable from "./ReportTable";
const { Title } = Typography;

const BookBorrowingReport = ({ reportQueryParameters }) => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    let isSetData = true;
    setLoading(true);
    reportServices
      .getBookBorrowingReport(reportQueryParameters)
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
      title: "Title",
      dataIndex: "title",
      key: "title",
    },
    {
      title: "Author",
      dataIndex: "author",
      key: "author",
    },
    {
      title: "Category",
      dataIndex: "category",
      key: "category",
    },
    {
      title: "Total Request",
      dataIndex: "totalBorrow",
      key: "totalRequest",
    },
    {
      title: "Quantity",
      dataIndex: "quantity",
      key: "quantity",
    },
    {
      title: "Available",
      dataIndex: "available",
      key: "available",
    },
  ];

  return (
    <>
      <ReportTable
        title={`📘 Report Top ${reportQueryParameters.top} Book Borrowing from
          ${reportQueryParameters.fromDate} to ${reportQueryParameters.toDate}`}
        columns={columns}
        loading={loading}
        data={data}
      ></ReportTable>
    </>
  );
};

export default BookBorrowingReport;
