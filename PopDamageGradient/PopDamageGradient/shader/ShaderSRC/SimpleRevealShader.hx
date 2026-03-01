package shaders;

import h3d.shader.ScreenShader;
import h3d.Vector;

class SimpleRevealShader extends ScreenShader {
	static var SRC = {
		@input var uv:Vec2;
		@param var texture:Sampler2D;
		@param var progress:Float = 0.0;
		@param var depth:Float = 1.0;
		@param var glowIntensity:Float = 0;
		function fragment() {
			// ===== Constant definition =====
			var BLACK_THRESHOLD:Float = 0.1;
			var COLOR_STEPS:Float = 10.0;
			var HUE_RANGE:Float = 0.5; // Red (0.0) to Cyan (0.5)
			var DEPTH_FACTOR_SCALE:Float = 0.15;
			var POWER_EXPONENT:Float = 0.8;
			var COLOR_BOOST:Float = 1.3;
			var GLOW_WEIGHT:Float = 0.25;
			var BASE_GLOW:Float = 0.2;
			var EDGE_GLOW_FACTOR:Float = 0.6;
			var ALPHA_BOOST:Float = 1.5;
			var EDGE_WIDTH:Float = 0.1;

			// ===== Texture sampling =====
			var texColor = texture.get(uv);

			// ===== Black Pixel Detection =====
			var isBlack:Bool = texColor.r < BLACK_THRESHOLD && texColor.g < BLACK_THRESHOLD && texColor.b < BLACK_THRESHOLD;

			// ===== Hotline Colour Calculation =====
			// Discrete hue transition: red → orange → yellow → green → cyan
			var stepIndex:Float = floor(progress * COLOR_STEPS);
			var hue:Float = stepIndex * (HUE_RANGE / COLOR_STEPS);
			var hsvColor:Vec3 = vec3(clamp(abs(hue * 6.0 - 3.0) - 1.0, 0.0, 1.0), clamp(2.0 - abs(hue * 6.0 - 2.0), 0.0, 1.0),
				clamp(2.0 - abs(hue * 6.0 - 4.0), 0.0, 1.0));

			// Depth effects and colour enhancement
			var depthFactor:Float = depth * DEPTH_FACTOR_SCALE;
			var hotlineColor:Vec3 = max(hsvColor - depthFactor, 0.0);
			hotlineColor = pow(hotlineColor, vec3(POWER_EXPONENT)) * COLOR_BOOST;

			// Halo effect
			var glowColor:Vec3 = vec3((hotlineColor.r + hotlineColor.b) * GLOW_WEIGHT) * (glowIntensity + BASE_GLOW);
			hotlineColor += glowColor;

			// ===== 透明度计算 =====
			var revealAlpha:Float = 1.0 - step(progress, uv.x); // Revealing regional transparency
			var edgeGlow:Float = 1.0 - smoothstep(0.0, EDGE_WIDTH, abs(uv.x - progress)); // Edge halo intensity
			var glowAlpha:Float = edgeGlow * EDGE_GLOW_FACTOR; // Edge Halo Transparency
			var combinedAlpha:Float = texColor.a * revealAlpha + glowAlpha; // Composite Transparency
			var finalAlpha:Float = clamp(combinedAlpha * ALPHA_BOOST, 0.0, 1.0); // Ultimate transparency

			// ===== Final colour output =====
			if (finalAlpha <= 0.0) {
				pixelColor = vec4(0.0, 0.0, 0.0, 0.0);
				return;
			}

			// Black pixels: Keep the main body black, with the edge halo using the hotline colour.
			if (isBlack) {
				var mixedColor = mix(texColor.rgb, hotlineColor, edgeGlow);
				pixelColor = vec4(mixedColor, finalAlpha);
			} else {
				pixelColor = vec4(hotlineColor, finalAlpha);
			}
		}
	}
}
