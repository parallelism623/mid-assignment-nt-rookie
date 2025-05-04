import React, { useState } from "react";
import { Card, DatePicker, Select, Divider } from "antd";
import {
  BookOutlined,
  PieChartOutlined,
  UserOutlined,
} from "@ant-design/icons";
import dayjs from "dayjs";
import { reportType } from "../../constants/reportType";
import { exportType } from "../../constants/exportType";
const { RangePicker } = DatePicker;
import { exportServices } from "../../services/exportServices";

const ReportCard = ({ onSelect }) => {
  const [selectedKey, setSelectedKey] = useState(reportType.bookBorrowing);
  const [timeOption, setTimeOption] = useState("7days");
  const [customRange, setCustomRange] = useState(null);
  const [topN, setTopN] = useState(10);

  const now = dayjs();

  const getDateRange = (timeOption = timeOption, customRange = customRange) => {
    if (timeOption === "custom" && customRange) {
      return {
        fromDate: customRange[0]?.format("YYYY-MM-DD"),
        toDate: customRange[1]?.format("YYYY-MM-DD"),
      };
    }

    const daysMap = {
      "7days": 7,
      "30days": 30,
      "365days": 365,
    };

    const days = daysMap[timeOption] || 7;
    return {
      fromDate: now.subtract(days, "day").format("YYYY-MM-DD"),
      toDate: now.format("YYYY-MM-DD"),
    };
  };
  const triggerOnReportFilterChange = (
    opt = options,
    timeOpt = timeOption,
    customRg = customRange,
    top = topN
  ) => {
    const dateRange = getDateRange(timeOpt, customRg);
    onSelect({
      option: opt,
      fromDate: dateRange.fromDate,
      toDate: dateRange.toDate,
      top: top,
    });
  };
  const handleSelect = (key) => {
    setSelectedKey(key);
    triggerOnReportFilterChange(key, timeOption, customRange, topN);
  };
  const handleOnDateOptionChange = (value) => {
    setTimeOption(value);
    triggerOnReportFilterChange(selectedKey, value, customRange, topN);
  };
  const handleOnDateRangeChange = (value) => {
    setCustomRange(value);
    triggerOnReportFilterChange(selectedKey, timeOption, value, topN);
  };
  const handleOnSelectTopChange = (value) => {
    setTopN(value);
    triggerOnReportFilterChange(selectedKey, timeOption, customRange, value);
  };
  const options = [
    {
      icon: <BookOutlined />,
      label: "Book Borrowing",
      key: reportType.bookBorrowing,
    },
    { icon: <PieChartOutlined />, label: "Category", key: reportType.category },
    {
      icon: <UserOutlined />,
      label: "User Engagement",
      key: reportType.userEngagement,
    },
  ];
  const onExport = (values) => {
    if (values.option == reportType.userEngagement) {
      exportServices.exportUserEngagementReport(values);
    }
    if (values.option == reportType.bookBorrowing) {
      exportServices.exportBookBorrowingReport(values);
    }
    if (values.option == reportType.category) {
      exportServices.exportCategoriesReport(values);
    }
  };
  return (
    <Card
      title="📊 Report Options"
      className="rounded-2xl shadow-lg mb-6"
      bodyStyle={{ padding: "20px" }}
    >
      <ul className="space-y-4 mb-6">
        {options.map((opt) => {
          const isSelected = selectedKey === opt.key;

          return (
            <li
              key={opt.key}
              onClick={() => handleSelect(opt.key)}
              className={`flex items-center space-x-3 px-3 py-2 rounded-lg cursor-pointer transition-all duration-150
                ${
                  isSelected
                    ? "bg-blue-100 text-blue-400 shadow-sm"
                    : "hover:bg-gray-300 hover:scale-[1.02]"
                }`}
            >
              <span className="text-lg">{opt.icon}</span>
              <span className="font-medium">{opt.label}</span>
            </li>
          );
        })}
      </ul>

      <Divider>Time</Divider>
      <Select
        className="w-full mb-3"
        value={timeOption}
        onChange={(value) => {
          handleOnDateOptionChange(value);
        }}
        options={[
          { label: "Last week", value: "7days" },
          { label: "Last month", value: "30days" },
          { label: "Last year", value: "365days" },
          { label: "Specific date", value: "custom" },
        ]}
      />

      {timeOption === "custom" && (
        <RangePicker
          className="w-full mb-3 !mt-5"
          onChange={(value) => {
            handleOnDateRangeChange(value);
          }}
        />
      )}

      <Divider>Top</Divider>
      <Select
        className="w-full"
        value={topN}
        onChange={(value) => {
          handleOnSelectTopChange(value);
        }}
        options={[
          { label: "Top 10", value: 10 },
          { label: "Top 20", value: 20 },
          { label: "Top 50", value: 50 },
        ]}
      />
      <Divider>Export</Divider>
      <div className="flex gap-4">
        <button
          className="px-4 py-2 bg-red-400 text-white rounded hover:bg-red-600 transition-all"
          onClick={() => {
            const { fromDate, toDate } = getDateRange(timeOption, customRange);
            onExport?.({
              fromDate,
              toDate,
              top: topN,
              option: selectedKey,
              exportType: exportType.pdf,
            });
          }}
        >
          Export PDF
        </button>
        <button
          className="px-4 py-2 bg-green-400 text-white rounded hover:bg-green-600 transition-all"
          onClick={() => {
            const { fromDate, toDate } = getDateRange(timeOption, customRange);
            onExport?.({
              fromDate,
              toDate,
              top: topN,
              option: selectedKey,
              exportType: exportType.excel,
            });
          }}
        >
          Export Excel
        </button>
      </div>
    </Card>
  );
};

export default ReportCard;
