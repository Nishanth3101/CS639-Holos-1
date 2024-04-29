import NavBar from "../NavBar/NavBar";
import "./aboutUs.css";
import Card from "../Card/Card";
import NishanthImage from "../assets/Nishanth.jpg";
import SakshamImage from "../assets/Saksham.jpg";
import WeibingImage from "../assets/Weibing.jpg";
import YuyangImage from "../assets/Yuyang.JPG";
import JordanImage from "../assets/Jordan.jpg"; // This line should correctly import the image

function AboutUs() {
  return (
    <>
      <NavBar />
      <header className="about-header">We're more than just a student team. We've got goals, hopes, and dreams, just like you...</header>
      <div className="team-container">
        <Card name="Nishanth Naik" description="Description Here" image={NishanthImage} />
        <Card name="Saksham Jain" description="Description Here" image={SakshamImage} />
        <Card name="Weibing Wang" description="Description Here" image={WeibingImage} />
        <Card name="Yuyang Liu" description="I'm a senior at the University of Wisconsin-Madison, and through VR, I explore new possibilities, and it's a good idea to make learning and entertainment come alive!" image={YuyangImage} />
        <Card name="Jordan Alper" description="Hi! My name’s Jordan Alper and I’m a senior at UW-Madison! I enjoy developing video games, so a VR project was a really cool experience for me, as it uses Unity as well!" image={JordanImage} />
      </div>
    </>
  );
}

export default AboutUs;
