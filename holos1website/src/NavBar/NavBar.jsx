import { Container, Nav, Navbar } from "react-bootstrap";
import "./navBar.css";

function NavBar() {
  return (
    <Navbar expand="lg" className="bg-body-tertiary">
      <Container className="nav-container">
        <Nav>
          <Nav.Link className="nav-item" href="/">Home</Nav.Link>
          <Nav.Link className="nav-item" href="/aboutus">About Us</Nav.Link>
          <Nav.Link className="nav-item" href="/demopage">Demo</Nav.Link>
          <Nav.Link className="nav-item" href="/quiz">Quiz</Nav.Link> 
        </Nav>
      </Container>
    </Navbar>
  );
}

export default NavBar;