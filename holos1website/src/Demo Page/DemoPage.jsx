import "./demoPage.css";
import Navbar from "../NavBar/NavBar";

function DemoPage() {
  return (
    <>
      <Navbar />
      <header className="demo-header">Demo Page</header>
      <div className="demo-content">
        <p>Check out our GitHub repository for more information:</p>
        <a href="https://github.com/Nishu3101/CS639-Holos-1" target="_blank" rel="noopener noreferrer">CS639-Holos-1 GitHub Repository</a>
      </div>
    </>
  );
}

export default DemoPage;