import {Layout} from "antd";
import AppHeader from "./AppHeader";
import AppFooter from "./AppFooter";
import AppContent from "./AppContent";

const MainLayout = () => {  
    return (<>
    <Layout style={{minHeight: "100vh"}}>
        <AppHeader/>
        <AppContent/>
        <AppFooter/>
    </Layout>
    </>)
}


export default MainLayout;