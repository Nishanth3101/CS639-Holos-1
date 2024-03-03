import NavBar from "../NavBar/NavBar";
import "./aboutUs.css";
import Card from "../Card/Card";
import image from "../assets/holos.jpg";

function AboutUs() {
  return (
    <>
      <NavBar />
      <header className="about-header">About Us Page</header>
      <div className="cards">
        <Card name="Jordan" description="Description Here" image={image} />
        <Card name="Nishanth" description="Description Here" image={image} />
        <Card name="Saksham" description="Description Here" image={image} />
        <Card name="Weibing" description="Description Here" image={image} />
        <Card name="Yuyang" description="Description Here" image={image} />
      </div>
    </>
  );
}

export default AboutUs;
